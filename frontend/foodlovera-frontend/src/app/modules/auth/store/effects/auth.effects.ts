import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, of, tap } from 'rxjs';
import { Router } from '@angular/router';

import { AuthApiService } from '../../services/auth-api.service';
import { AuthTokenStorage } from '../../services/auth-token.storage';
import { AuthActions } from '../actions/auth.actions';
import { Action } from '@ngrx/store';
import { Store } from '@ngrx/store';
import { withLatestFrom } from 'rxjs/operators';
import { selectRequiresEmailVerification } from '../selectors/auth.selectors';

@Injectable()
export class AuthEffects {
  private readonly actions$ = inject(Actions);
  private readonly api = inject(AuthApiService);
  private readonly storage = inject(AuthTokenStorage);
  private readonly router = inject(Router);
  private readonly store = inject(Store);

  hydrateFromStorage$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.hydrateFromStorage),
      map(() => {
        const token = this.storage.get();
        return AuthActions.authSucceeded({
          response: { accessToken: token, requiresEmailVerification: false },
        });
      })
    )
  );

  login$ = createEffect(() =>
  this.actions$.pipe(
    ofType(AuthActions.loginRequested),
    mergeMap(({ request }) =>
      this.api.login(request).pipe(
        mergeMap((response) => {
          const actions: Action[] = [AuthActions.authSucceeded({ response })];

          if (response.requiresEmailVerification) {
            actions.push(AuthActions.setPendingVerificationEmail({ email: request.email }));
          }

          return actions;
        }),
        catchError((err) => of(AuthActions.authFailed({ error: this.extractError(err) })))
      )
    )
  )
);

  register$ = createEffect(() =>
  this.actions$.pipe(
    ofType(AuthActions.registerRequested),
    mergeMap(({ request }) =>
      this.api.register(request).pipe(
        mergeMap((response) =>
          response.requiresEmailVerification
            ? [
              AuthActions.authSucceeded({ response }),
              AuthActions.setPendingVerificationEmail({ email: request.email }),
              ]
            : [AuthActions.authSucceeded({ response })]
          ),
        catchError((err) =>
          of(AuthActions.authFailed({ error: this.extractError(err) }))
        )
      )
    )
  )
);
  verifyEmail$ = createEffect(() =>
  this.actions$.pipe(
    ofType(AuthActions.verifyEmailRequested),
    mergeMap(({ email, code }) =>
      this.api.verifyEmail({ email, code }).pipe(
        map(() => AuthActions.verifyEmailSucceeded()),
        catchError((err) =>
          of(AuthActions.verifyEmailFailed({ error: this.extractError(err) }))
)
      )
    )
  )
);
  resend$ = createEffect(() =>
  this.actions$.pipe(
    ofType(AuthActions.resendVerificationRequested),
    mergeMap(({ email }) =>
      this.api.resendVerification({ email }).pipe(
        map(() => AuthActions.clearError()),
        catchError((err) => of(AuthActions.authFailed({ error: this.extractError(err) })))
      )
    )
  )
);

  persistTokenAndCloseModal$ = createEffect(
  () =>
    this.actions$.pipe(
      ofType(AuthActions.authSucceeded),

      withLatestFrom(this.store.select(selectRequiresEmailVerification)),

      tap(([{ response }, requiredBefore]) => {
        const token = response.accessToken;

        if (token) {
          this.storage.set(token);
        }

        const verifiedNow = requiredBefore && !response.requiresEmailVerification;

        // dacă tocmai s-a verificat emailul, NU închidem modalul
        if (token && !verifiedNow) {
          this.router.navigate([{ outlets: { modal: null } }]);
        }
      })
    ),
  { dispatch: false }
);

  logout$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.logout),
        tap(() => this.storage.clear())
      ),
    { dispatch: false }
  );

  private extractError(err: any): string {
  const raw =
    (err?.error?.detail as string | undefined) ??
    (err?.error?.title as string | undefined) ??
    (err?.message as string | undefined) ??
    '';

  const msg = raw.toLowerCase();

  // Password policy 
  if (msg.includes('weak password') || (msg.includes('password') && msg.includes('uppercase'))) {
    return 'auth.errors.weakPassword';
  }

  // Whitespace 
  if (msg.includes('whitespace') || msg.includes('only spaces') || msg.includes('empty')) {
    return 'auth.errors.invalidInput';
  }

  // Username taken
  if (msg.includes('username') && (msg.includes('taken') || msg.includes('already'))) {
    return 'auth.errors.usernameTaken';
  }

  // Email already used
  if (msg.includes('email') && (msg.includes('taken') || msg.includes('already') || msg.includes('exists'))) {
    return 'auth.errors.emailTaken';
  }

  // Invalid credentials
  if (msg.includes('invalid credentials') || (msg.includes('email') && msg.includes('password') && msg.includes('invalid'))) {
    return 'auth.errors.invalidCredentials';
  }

  // Fallback generic
  return 'auth.errors.generic';
}
}