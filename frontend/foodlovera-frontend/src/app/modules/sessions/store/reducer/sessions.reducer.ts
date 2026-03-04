import { createReducer, on } from '@ngrx/store';
import * as SessionsActions from '../actions/sessions.actions';
import { SessionsState, initialSessionsState } from './sessions.state';

export const sessionsFeatureKey = 'sessions';

export const sessionsReducer = createReducer<SessionsState>(
  initialSessionsState,

  on(SessionsActions.createSession, (state) => ({
    ...state,
    isLoading: true,
    error: null,
  })),

  on(SessionsActions.createSessionSuccess, (state, { sessionId, joinCode, name }) => ({
    ...state,
    sessionId,
    joinCode,
    sessionName: name,
    isLoading: false,
    error: null,
  })),

  on(SessionsActions.createSessionFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error,
  })),

  on(SessionsActions.clearSession, () => initialSessionsState)
);