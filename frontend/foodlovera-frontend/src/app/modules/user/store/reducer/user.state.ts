export interface UserState {
  username: string | null;

  isLoading: boolean;
  error: unknown | null;
}

export const initialUserState: UserState = {
  username: null,
  isLoading: false,
  error: null,
};