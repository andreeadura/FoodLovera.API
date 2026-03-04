import { createFeatureSelector, createSelector } from '@ngrx/store';
import { authFeatureKey } from '../reducer/auth.reducer';
import { AuthState } from '../reducer/auth.state';

export const selectAuthState =
  createFeatureSelector<AuthState>(authFeatureKey);

export const selectAccessToken = createSelector(
  selectAuthState,
  (state) => state.accessToken
);

export const selectAuthLoading = createSelector(
  selectAuthState,
  (state) => state.isLoading
);

export const selectAuthError = createSelector(
  selectAuthState,
  (state) => state.error
);

export const selectIsAuthenticated = createSelector(
  selectAccessToken,
  (token) => !!token
);