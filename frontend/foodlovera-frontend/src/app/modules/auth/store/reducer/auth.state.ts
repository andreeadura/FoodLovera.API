export interface AuthState {
  accessToken: string | null;
  isLoading: boolean;
  error: unknown | null;
}

export const initialAuthState: AuthState = {
  accessToken: null,
  isLoading: false,
  error: null,
};