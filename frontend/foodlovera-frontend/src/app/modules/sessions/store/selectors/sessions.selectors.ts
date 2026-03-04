import { createFeatureSelector, createSelector } from '@ngrx/store';
import { sessionsFeatureKey } from '../reducer/sessions.reducer';
import { SessionsState } from '../reducer/sessions.state';

export const selectSessionsState =
  createFeatureSelector<SessionsState>(sessionsFeatureKey);

export const selectSessionId = createSelector(
  selectSessionsState,
  (state) => state.sessionId
);

export const selectJoinCode = createSelector(
  selectSessionsState,
  (state) => state.joinCode
);

export const selectSessionsLoading = createSelector(
  selectSessionsState,
  (state) => state.isLoading
);

export const selectSessionsError = createSelector(
  selectSessionsState,
  (state) => state.error
);