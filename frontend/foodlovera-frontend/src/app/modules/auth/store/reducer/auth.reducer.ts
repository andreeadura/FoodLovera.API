import { createReducer, on } from '@ngrx/store';
import * as AuthActions from '../actions/auth.actions';
import { AuthState, initialAuthState } from './auth.state';

export const authFeatureKey = 'auth';

export const authReducer = createReducer<AuthState>(
  initialAuthState,

  on(AuthActions.login, (state) => ({
    ...state,
    isLoading: true,
    error: null,
  })),

  on(AuthActions.loginSuccess, (state, { accessToken }) => ({
    ...state,
    accessToken,
    isLoading: false,
    error: null,
  })),

  on(AuthActions.loginFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error,
  })),

  on(AuthActions.logout, (state) => ({
    ...state,
    accessToken: null,
    error: null,
  }))
);