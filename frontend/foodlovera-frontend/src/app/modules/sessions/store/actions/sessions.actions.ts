import { createAction, props } from '@ngrx/store';

export const createSession = createAction(
  '[Sessions] Create Session',
  props<{ name: string }>()
);

export const createSessionSuccess = createAction(
  '[Sessions] Create Session Success',
  props<{ sessionId: string; joinCode: string; name: string }>()
);

export const createSessionFailure = createAction(
  '[Sessions] Create Session Failure',
  props<{ error: string }>()
);

export const clearSession = createAction('[Sessions] Clear Session');