import { createAction, props } from '@ngrx/store';

export const setUsername = createAction(
  '[User] Set Username',
  props<{ username: string }>()
);

export const changeUsername = createAction(
  '[User] Change Username',
  props<{ username: string }>()
);

export const changeUsernameSuccess = createAction(
  '[User] Change Username Success',
  props<{ username: string }>()
);

export const changeUsernameFailure = createAction(
  '[User] Change Username Failure',
  props<{ error: unknown }>()
);

export const clearUserState = createAction('[User] Clear State');