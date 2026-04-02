import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  DestroyRef,
  OnInit,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { TranslateModule } from '@ngx-translate/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import {
  GameRestaurant,
  GameStateResponse,
  SessionApiService,
  WinnerResponse,
} from '../services/session-api.service';

@Component({
  selector: 'app-session-next',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './session-next.component.html',
  styleUrls: ['./session-next.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SessionNextComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly sessionApi = inject(SessionApiService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly destroyRef = inject(DestroyRef);

  readonly sessionId = Number(this.route.snapshot.paramMap.get('sessionId'));
  readonly participantId = Number(
    this.route.snapshot.queryParamMap.get('participantId')
  );

  currentRestaurant: GameRestaurant | null = null;
  winners: WinnerResponse[] = [];

  isLoading = true;
  isCompleted = false;
  isSubmittingVote = false;
  errorKey: string | null = null;

  isUnanimousMatch = false;
  myVoteIsLike: boolean | null = null;

  roundNumber = 0;
  roundEndsAtUtc: string | null = null;
  remainingSeconds = 0;

  private serverOffsetMs = 0;
  private countdownHandle: ReturnType<typeof setInterval> | null = null;
  private pollHandle: ReturnType<typeof setInterval> | null = null;
  private isPolling = false;

  ngOnInit(): void {
    if (
      !this.sessionId ||
      Number.isNaN(this.sessionId) ||
      !this.participantId ||
      Number.isNaN(this.participantId)
    ) {
      this.errorKey = 'sessionNext.errors.invalidSession';
      this.isLoading = false;
      return;
    }

    this.destroyRef.onDestroy(() => {
      this.stopCountdown();
      this.stopPolling();
    });

    this.loadState(true);
  }

  get isLikeSelected(): boolean {
    return this.myVoteIsLike === true;
  }

  get isDislikeSelected(): boolean {
    return this.myVoteIsLike === false;
  }

  get hasRestaurant(): boolean {
    return !!this.currentRestaurant && !this.isCompleted;
  }

  selectLike(): void {
    this.saveVote(true);
  }

  selectDislike(): void {
    this.saveVote(false);
  }

  private loadState(initial = false): void {
    if (initial) {
      this.isLoading = true;
    }

    this.sessionApi
      .getGameState(this.sessionId, this.participantId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.applyState(response);
          this.isLoading = false;
          this.errorKey = null;
          this.cdr.markForCheck();
        },
        error: (err: HttpErrorResponse) => {
          this.isLoading = false;
          this.handleError(err);
        },
      });
  }

  private applyState(response: GameStateResponse): void {
    this.isCompleted = response.completed;
    this.isUnanimousMatch = response.isUnanimousMatch;
    this.winners = response.winners ?? [];
    this.currentRestaurant = response.currentRestaurant;
    this.myVoteIsLike = response.myVoteIsLike;
    this.roundNumber = response.roundNumber;
    this.roundEndsAtUtc = response.roundEndsAtUtc;

    this.serverOffsetMs =
      new Date(response.serverUtcNow).getTime() - Date.now();

    if (response.completed || !response.roundEndsAtUtc) {
      this.remainingSeconds = 0;
      this.stopCountdown();
      this.stopPolling();
      return;
    }

    this.startCountdown();
    this.startPolling();
  }

  private saveVote(isLike: boolean): void {
    if (this.isCompleted || this.isSubmittingVote || !this.currentRestaurant) {
      return;
    }

    this.isSubmittingVote = true;
    this.myVoteIsLike = isLike;
    this.cdr.markForCheck();

    this.sessionApi
      .setVote(this.sessionId, {
        participantId: this.participantId,
        isLike,
      })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.isSubmittingVote = false;
          this.applyState(response);
          this.cdr.markForCheck();
        },
        error: (err: HttpErrorResponse) => {
          this.isSubmittingVote = false;
          this.handleError(err);
        },
      });
  }

  private startCountdown(): void {
    this.stopCountdown();
    this.updateRemainingSeconds();

    this.countdownHandle = setInterval(() => {
      this.updateRemainingSeconds();
      this.cdr.markForCheck();
    }, 250);
  }

  private updateRemainingSeconds(): void {
    if (!this.roundEndsAtUtc) {
      this.remainingSeconds = 0;
      return;
    }

    const serverNowMs = Date.now() + this.serverOffsetMs;
    const endsAtMs = new Date(this.roundEndsAtUtc).getTime();
    const diffMs = Math.max(0, endsAtMs - serverNowMs);

    this.remainingSeconds = Math.ceil(diffMs / 1000);
  }

  private stopCountdown(): void {
    if (this.countdownHandle) {
      clearInterval(this.countdownHandle);
      this.countdownHandle = null;
    }
  }

  private startPolling(): void {
    if (this.pollHandle) {
      return;
    }

    this.pollHandle = setInterval(() => {
      if (this.isPolling) {
        return;
      }

      this.isPolling = true;

      this.sessionApi
        .getGameState(this.sessionId, this.participantId)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: (response) => {
            this.isPolling = false;
            const oldRound = this.roundNumber;
            const oldCompleted = this.isCompleted;

            this.applyState(response);

            if (
              response.completed ||
              response.roundNumber !== oldRound ||
              response.completed !== oldCompleted
            ) {
              this.cdr.markForCheck();
            }
          },
          error: () => {
            this.isPolling = false;
          },
        });
    }, 1000);
  }

  private stopPolling(): void {
    if (this.pollHandle) {
      clearInterval(this.pollHandle);
      this.pollHandle = null;
    }

    this.isPolling = false;
  }

  private handleError(err: HttpErrorResponse): void {
    const errorBody = err.error as
      | {
          messageKey?: string;
          detailKey?: string;
          message?: string;
          detail?: string;
        }
      | null
      | undefined;

    this.errorKey =
      errorBody?.messageKey ??
      errorBody?.detailKey ??
      errorBody?.message ??
      errorBody?.detail ??
      'sessionNext.errors.generic';

    this.cdr.markForCheck();
  }

  trackWinner(_: number, winner: WinnerResponse): number {
    return winner.restaurantId;
  }

  trackCategory(_: number, category: string): string {
    return category;
  }

  goHome(): void {
    this.router.navigate(['/']);
  }
}