import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Action, Store } from '@ngrx/store';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, of, tap, withLatestFrom } from 'rxjs';

import { AuthApiService } from '../../services/auth-api.service';
import { AuthTokenStorage } from '../../services/auth-token.storage';
import { AuthActions } from '../actions/auth.actions';
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

        if (!token) {
          return AuthActions.logout();
        }

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
          catchError((err) => of(AuthActions.authFailed({ error: this.extractError(err) })))
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
        tap(() => {
          this.storage.clear();
          this.router.navigateByUrl('/');
        })
      ),
    { dispatch: false }
  );

  private extractError(err: unknown): string {
    const error = err as {
      error?: { detail?: string; title?: string };
      message?: string;
    };

    const raw =
      error?.error?.detail ??
      error?.error?.title ??
      error?.message ??
      '';

    const msg = raw.toLowerCase();

    if (msg.includes('weak password') || (msg.includes('password') && msg.includes('uppercase'))) {
      return 'auth.errors.weakPassword';
    }

    if (msg.includes('whitespace') || msg.includes('only spaces') || msg.includes('empty')) {
      return 'auth.errors.invalidInput';
    }

    if (msg.includes('username') && (msg.includes('taken') || msg.includes('already'))) {
      return 'auth.errors.usernameTaken';
    }

    if (msg.includes('email') && (msg.includes('taken') || msg.includes('already') || msg.includes('exists'))) {
      return 'auth.errors.emailTaken';
    }

    if (msg.includes('invalid credentials') || (msg.includes('email') && msg.includes('password') && msg.includes('invalid'))) {
      return 'auth.errors.invalidCredentials';
    }

    return 'auth.errors.generic';
  }
}