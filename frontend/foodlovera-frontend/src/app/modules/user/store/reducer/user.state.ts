import { UserProfile } from '../../services/user-api.service';

export interface UserState {
  profile: UserProfile | null;
  isLoading: boolean;
  error: string | null;
  usernameUpdated: boolean;
  passwordChanged: boolean;
}

export const initialUserState: UserState = {
  profile: null,
  isLoading: false,
  error: null,
  usernameUpdated: false,
  passwordChanged: false,
};