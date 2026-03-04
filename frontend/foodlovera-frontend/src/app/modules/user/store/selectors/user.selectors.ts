import { createFeatureSelector, createSelector } from '@ngrx/store';
import { userFeatureKey } from '../reducer/user.reducer';
import { UserState } from '../reducer/user.state';

export const selectUserState =
  createFeatureSelector<UserState>(userFeatureKey);

export const selectUsername = createSelector(
  selectUserState,
  (state) => state.username
);

export const selectUserLoading = createSelector(
  selectUserState,
  (state) => state.isLoading
);

export const selectUserError = createSelector(
  selectUserState,
  (state) => state.error
);