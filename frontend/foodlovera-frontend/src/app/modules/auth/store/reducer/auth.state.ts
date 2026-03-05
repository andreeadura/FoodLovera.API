export interface AuthState {
  accessToken: string | null;
  isLoading: boolean;
  error: string | null;
  verificationSuccess: boolean;
}

export const initialAuthState: AuthState = {
  accessToken: null,
  isLoading: false,
  error: null,
  verificationSuccess: false,
};