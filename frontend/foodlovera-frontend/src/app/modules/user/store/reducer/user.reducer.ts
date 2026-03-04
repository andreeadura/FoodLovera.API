import { createReducer, on } from '@ngrx/store';
import * as UserActions from '../actions/user.actions';
import { UserState, initialUserState } from './user.state';

export const userFeatureKey = 'user';

export const userReducer = createReducer<UserState>(
  initialUserState,

  on(UserActions.setUsername, (state, { username }) => ({
    ...state,
    username,
  })),

  on(UserActions.changeUsername, (state) => ({
    ...state,
    isLoading: true,
    error: null,
  })),

  on(UserActions.changeUsernameSuccess, (state, { username }) => ({
    ...state,
    username,
    isLoading: false,
    error: null,
  })),

  on(UserActions.changeUsernameFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error,
  })),

  on(UserActions.clearUserState, () => initialUserState)
);