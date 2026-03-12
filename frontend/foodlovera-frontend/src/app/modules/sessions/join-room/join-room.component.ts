import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { finalize } from 'rxjs';

import { AuthTokenStorage } from '../../auth/services/auth-token.storage';
import { SessionApiService } from '../services/session-api.service';

@Component({
  selector: 'app-join-room',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './join-room.component.html',
  styleUrls: ['./join-room.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class JoinRoomComponent {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly tokenStorage = inject(AuthTokenStorage);
  private readonly sessionsApi = inject(SessionApiService);

  readonly isAuthenticated = !!this.tokenStorage.get();

  isSubmitting = false;
  serverError: string | null = null;

  readonly form = this.fb.nonNullable.group({
    code: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
    username: ['', this.isAuthenticated ? [] : [Validators.required, Validators.minLength(2)]],
  });

  get codeControl() {
    return this.form.controls.code;
  }

  get usernameControl() {
    return this.form.controls.username;
  }

  onCodeInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const onlyDigits = input.value.replace(/\D/g, '').slice(0, 6);

    input.value = onlyDigits;
    this.codeControl.setValue(onlyDigits, { emitEvent: false });
  }

  submit(): void {
    this.serverError = null;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { code, username } = this.form.getRawValue();

    this.isSubmitting = true;

    const request$ = this.isAuthenticated
      ? this.sessionsApi.joinAsAuthenticated(code)
      : this.sessionsApi.joinAsGuest(code, {
          displayName: username.trim(),
        });

    request$
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: (response) => {
          this.router.navigate(['/sessions', response.sessionId], {
            queryParams: {
              participantId: response.participantId,
            },
          });
        },
        error: (err) => {
          this.serverError =
            err?.error?.message ??
            err?.error?.detail ??
            'Something went wrong. Please try again.';
        },
      });
  }
}