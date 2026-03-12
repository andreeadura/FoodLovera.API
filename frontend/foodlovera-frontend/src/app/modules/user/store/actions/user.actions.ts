import { createAction, props } from '@ngrx/store';
import {
  ChangePasswordRequest,
  UpdateUsernameRequest,
  UserProfile,
} from '../../services/user-api.service';

export const loadProfile = createAction('[User] Load Profile');

export const loadProfileSuccess = createAction(
  '[User] Load Profile Success',
  props<{ profile: UserProfile }>()
);

export const loadProfileFailure = createAction(
  '[User] Load Profile Failure',
  props<{ error: string }>()
);

export const updateUsername = createAction(
  '[User] Update Username',
  props<{ request: UpdateUsernameRequest }>()
);

export const updateUsernameSuccess = createAction(
  '[User] Update Username Success'
);

export const updateUsernameFailure = createAction(
  '[User] Update Username Failure',
  props<{ error: string }>()
);

export const changePassword = createAction(
  '[User] Change Password',
  props<{ request: ChangePasswordRequest }>()
);

export const changePasswordSuccess = createAction(
  '[User] Change Password Success'
);

export const changePasswordFailure = createAction(
  '[User] Change Password Failure',
  props<{ error: string }>()
);

export const clearUserError = createAction('[User] Clear Error');
export const clearUserStatus = createAction('[User] Clear Status');
export const clearUserState = createAction('[User] Clear State');