import { createFeatureSelector, createSelector } from '@ngrx/store';
import { adminFeatureKey } from '../reducer/admin.reducer';
import { AdminState } from '../reducer/admin.state';

export const selectAdminState =
  createFeatureSelector<AdminState>(adminFeatureKey);

export const selectCities = createSelector(
  selectAdminState,
  (state) => state.cities
);

export const selectRestaurants = createSelector(
  selectAdminState,
  (state) => state.restaurants
);

export const selectAdminUsers = createSelector(
  selectAdminState,
  (state) => state.users
);

export const selectAdminLoading = createSelector(
  selectAdminState,
  (state) => state.isLoading
);

export const selectAdminError = createSelector(
  selectAdminState,
  (state) => state.error
);