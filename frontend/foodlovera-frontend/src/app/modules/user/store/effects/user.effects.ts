import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, of } from 'rxjs';

import { UserApiService } from '../../services/user-api.service';
import * as UserActions from '../actions/user.actions';

@Injectable()
export class UserEffects {
  private readonly actions$ = inject(Actions);
  private readonly api = inject(UserApiService);

  loadProfile$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.loadProfile),
      mergeMap(() =>
        this.api.getMyProfile().pipe(
          map((profile) => UserActions.loadProfileSuccess({ profile })),
          catchError((err) =>
            of(UserActions.loadProfileFailure({ error: this.extractError(err) }))
          )
        )
      )
    )
  );

  updateUsername$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.updateUsername),
      mergeMap(({ request }) =>
        this.api.updateUsername(request).pipe(
          map(() => UserActions.updateUsernameSuccess()),
          catchError((err) =>
            of(
              UserActions.updateUsernameFailure({
                error: this.extractError(err),
              })
            )
          )
        )
      )
    )
  );

  reloadProfileAfterUsernameUpdate$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.updateUsernameSuccess),
      map(() => UserActions.loadProfile())
    )
  );

  changePassword$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserActions.changePassword),
      mergeMap(({ request }) =>
        this.api.changePassword(request).pipe(
          map(() => UserActions.changePasswordSuccess()),
          catchError((err) =>
            of(
              UserActions.changePasswordFailure({
                error: this.extractError(err),
              })
            )
          )
        )
      )
    )
  );

private extractError(err: unknown): string {
  const error = err as {
    error?: { detail?: string; title?: string };
    message?: string;
  };

  return (
    error?.error?.detail ??
    error?.error?.title ??
    error?.message ??
    'errors.generic'
  );
}
}