import { createReducer, on } from '@ngrx/store';
import { AuthActions } from '../../../auth/store/actions/auth.actions';
import * as UserActions from '../actions/user.actions';
import { UserState, initialUserState } from './user.state';

export const userFeatureKey = 'user';

export const userReducer = createReducer<UserState>(
  initialUserState,

  on(
    UserActions.loadProfile,
    UserActions.updateUsername,
    UserActions.changePassword,
    (state) => ({
      ...state,
      isLoading: true,
      error: null,
      usernameUpdated: false,
      passwordChanged: false,
    })
  ),

  on(UserActions.loadProfileSuccess, (state, { profile }) => ({
    ...state,
    profile,
    isLoading: false,
    error: null,
  })),

  on(UserActions.updateUsernameSuccess, (state) => ({
    ...state,
    isLoading: false,
    error: null,
    usernameUpdated: true,
  })),

  on(UserActions.changePasswordSuccess, (state) => ({
    ...state,
    isLoading: false,
    error: null,
    passwordChanged: true,
  })),

  on(
    UserActions.loadProfileFailure,
    UserActions.updateUsernameFailure,
    UserActions.changePasswordFailure,
    (state, { error }) => ({
      ...state,
      isLoading: false,
      error,
    })
  ),

  on(UserActions.clearUserError, (state) => ({
    ...state,
    error: null,
  })),

  on(UserActions.clearUserStatus, (state) => ({
    ...state,
    usernameUpdated: false,
    passwordChanged: false,
  })),

  on(UserActions.clearUserState, AuthActions.logout, () => ({
    ...initialUserState,
  }))
);