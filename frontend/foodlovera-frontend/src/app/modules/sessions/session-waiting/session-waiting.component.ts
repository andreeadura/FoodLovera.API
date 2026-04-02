import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { interval, of } from 'rxjs';
import { catchError, startWith, switchMap, tap } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import {
  SessionApiService,
  SessionStatusResponse,
} from '../services/session-api.service';

@Component({
  selector: 'app-session-waiting',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './session-waiting.component.html',
  styleUrls: ['./session-waiting.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SessionWaitingComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly sessionsApi = inject(SessionApiService);
  private readonly destroyRef = inject(DestroyRef);

  readonly sessionId = Number(this.route.snapshot.paramMap.get('sessionId'));
  readonly participantId = this.route.snapshot.queryParamMap.get('participantId');

  status: SessionStatusResponse | null = null;
  isLoading = true;
  errorKey: string | null = null;
  allParticipantsJoined = false;
  private hasRedirected = false;

  constructor() {
    if (!this.sessionId || Number.isNaN(this.sessionId)) {
      this.errorKey = 'waitingRoom.errors.invalidSession';
      this.isLoading = false;
      return;
    }

    interval(3000)
      .pipe(
        startWith(0),
        switchMap(() =>
          this.sessionsApi.getStatus(this.sessionId).pipe(
            catchError(() => {
              this.errorKey = 'waitingRoom.errors.loadFailed';
              this.isLoading = false;
              return of(null);
            })
          )
        ),
        tap((status) => {
          if (!status) {
            return;
          }

          this.status = status;
          this.isLoading = false;
          this.errorKey = null;
          this.allParticipantsJoined =
            status.currentParticipants >= status.requiredParticipants;

          if (this.allParticipantsJoined && !this.hasRedirected) {
            this.hasRedirected = true;

            this.router.navigate(['/sessions', this.sessionId, 'next'], {
              queryParams: this.participantId
                ? { participantId: this.participantId }
                : undefined,
            });
          }
        }),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe();
  }
}