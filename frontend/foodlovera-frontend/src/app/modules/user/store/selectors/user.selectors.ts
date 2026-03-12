import { createFeatureSelector, createSelector } from '@ngrx/store';
import { userFeatureKey } from '../reducer/user.reducer';
import { UserState } from '../reducer/user.state';

export const selectUserState =
  createFeatureSelector<UserState>(userFeatureKey);

export const selectUserProfile = createSelector(
  selectUserState,
  (state) => state.profile
);

export const selectUserLoading = createSelector(
  selectUserState,
  (state) => state.isLoading
);

export const selectUserError = createSelector(
  selectUserState,
  (state) => state.error
);

export const selectUsernameUpdated = createSelector(
  selectUserState,
  (state) => state.usernameUpdated
);

export const selectPasswordChanged = createSelector(
  selectUserState,
  (state) => state.passwordChanged
);