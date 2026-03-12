export interface AuthState {
  accessToken: string | null;
  isAuthenticated: boolean;
  loading: boolean;
  error: string | null;
  requiresEmailVerification: boolean;
  pendingVerificationEmail: string | null;
  verificationSuccess: boolean;
}

export const initialAuthState: AuthState = {
  accessToken: null,
  isAuthenticated: false,
  loading: false,
  error: null,
  requiresEmailVerification: false,
  pendingVerificationEmail: null,
  verificationSuccess: false,
};