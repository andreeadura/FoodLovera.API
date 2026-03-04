import { createReducer, on } from '@ngrx/store';
import * as AdminActions from '../actions/admin.actions';
import { AdminState, initialAdminState } from './admin.state';

export const adminFeatureKey = 'admin';

export const adminReducer = createReducer<AdminState>(
  initialAdminState,

  // Cities
  on(AdminActions.loadCities, (state) => ({ ...state, isLoading: true, error: null })),
  on(AdminActions.loadCitiesSuccess, (state, { cities }) => ({
    ...state,
    cities,
    isLoading: false,
    error: null,
  })),
  on(AdminActions.loadCitiesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error,
  })),

  // Restaurants
  on(AdminActions.loadRestaurants, (state) => ({ ...state, isLoading: true, error: null })),
  on(AdminActions.loadRestaurantsSuccess, (state, { restaurants }) => ({
    ...state,
    restaurants,
    isLoading: false,
    error: null,
  })),
  on(AdminActions.loadRestaurantsFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error,
  })),

  // Users
  on(AdminActions.loadUsers, (state) => ({ ...state, isLoading: true, error: null })),
  on(AdminActions.loadUsersSuccess, (state, { users }) => ({
    ...state,
    users,
    isLoading: false,
    error: null,
  })),
  on(AdminActions.loadUsersFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error,
  })),

  on(AdminActions.clearAdminState, () => initialAdminState)
);