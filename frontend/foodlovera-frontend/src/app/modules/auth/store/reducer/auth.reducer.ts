import { createReducer, on } from '@ngrx/store';
import { AuthActions } from '../actions/auth.actions';

export const authFeatureKey = 'auth';

export interface AuthState {
  accessToken: string | null;
  isAuthenticated: boolean;
  loading: boolean;
  error: string | null;

  requiresEmailVerification: boolean;
  pendingVerificationEmail: string | null;

  verificationSuccess: boolean;
}

export const initialState: AuthState = {
  accessToken: null,
  isAuthenticated: false,
  loading: false,
  error: null,

  requiresEmailVerification: false,
  pendingVerificationEmail: null,

  verificationSuccess: false,
};

export const authReducer = createReducer(
  initialState,

  on(AuthActions.hydrateFromStorage, (state) => ({
    ...state,
    error: null,
  })),

  on(AuthActions.loginRequested, AuthActions.registerRequested, (state) => ({
    ...state,
    loading: true,
    error: null,
    verificationSuccess: false,
  })),

  on(AuthActions.verifyEmailRequested, (state) => ({
    ...state,
    loading: true,
    error: null,
    verificationSuccess: false,
  })),

  on(AuthActions.verifyEmailSucceeded, (state) => ({
  ...state,
  loading: false,
  error: null,
  requiresEmailVerification: false,
  verificationSuccess: true,
})),

  on(AuthActions.verifyEmailFailed, (state, { error }) => ({
  ...state,
  loading: false,
  error,
  verificationSuccess: false,
})),

  on(AuthActions.authSucceeded, (state, { response }) => {
    const token = response.accessToken ?? null;

    // success = doar dacă înainte cerea verificare și acum nu mai cere
    const verifiedNow = state.requiresEmailVerification && !response.requiresEmailVerification;

    const requiresVerification = response.requiresEmailVerification === true;

    return {
      ...state,
      loading: false,
      error: null,

      accessToken: token,
      isAuthenticated: !!token,

      requiresEmailVerification: requiresVerification,

      // dacă backend cere verificare, păstrează email-ul (dacă îl ai deja setat)
      pendingVerificationEmail: requiresVerification ? state.pendingVerificationEmail : state.pendingVerificationEmail,

      // aprinde success doar la tranziția de verify
      verificationSuccess: verifiedNow,
    };
  }),

  on(AuthActions.authFailed, (state, { error }) => ({
    ...state,
    loading: false,
    error,
    verificationSuccess: false,
  })),

  on(AuthActions.logout, () => ({
    ...initialState,
  })),

  on(AuthActions.clearError, (state) => ({
    ...state,
    error: null,
  })),

  on(AuthActions.setPendingVerificationEmail, (state, { email }) => ({
    ...state,
    pendingVerificationEmail: email,
  })),

  on(AuthActions.clearVerificationFlow, (state) => ({
    ...state,
    pendingVerificationEmail: null,
    requiresEmailVerification: false,
    verificationSuccess: false,
    error: null,
    loading: false,
  }))
);