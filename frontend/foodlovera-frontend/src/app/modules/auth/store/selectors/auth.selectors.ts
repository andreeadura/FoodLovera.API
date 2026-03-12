import { createFeatureSelector, createSelector } from '@ngrx/store';
import { authFeatureKey, AuthState } from '../reducer/auth.reducer';

export const selectAuthState = createFeatureSelector<AuthState>(authFeatureKey);

export const selectIsAuthenticated = createSelector(
  selectAuthState,
  (s) => s.isAuthenticated
);

export const selectAuthLoading = createSelector(
  selectAuthState,
  (s) => s.loading
);

export const selectAuthError = createSelector(
  selectAuthState,
  (s) => s.error
);

export const selectRequiresEmailVerification = createSelector(
  selectAuthState,
  (s) => s.requiresEmailVerification
);

export const selectPendingVerificationEmail = createSelector(
  selectAuthState,
  (s) => s.pendingVerificationEmail
);

export const selectNeedsEmailVerification = createSelector(
  selectAuthState,
  (s) => s.requiresEmailVerification && !s.isAuthenticated
);

export const selectVerificationSuccess = createSelector(
  selectAuthState,
  (s) => s.verificationSuccess
);