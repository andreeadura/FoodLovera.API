import { createReducer, on } from '@ngrx/store';
import * as AdminActions from '../actions/admin.actions';
import { AdminState, initialAdminState } from './admin.state';

export const adminFeatureKey = 'admin';

export const adminReducer = createReducer<AdminState>(
  initialAdminState,

  on(
    AdminActions.loadCities,
    AdminActions.loadRestaurants,
    AdminActions.loadUsers,
    AdminActions.createCity,
    AdminActions.createRestaurant,
    AdminActions.deleteCity,
    AdminActions.deleteRestaurant,
    AdminActions.deleteUser,
    (state) => ({
      ...state,
      isLoading: true,
      error: null,
    })
  ),

  on(AdminActions.loadCitiesSuccess, (state, { cities }) => ({
    ...state,
    cities,
    isLoading: false,
    error: null,
  })),

  on(AdminActions.loadRestaurantsSuccess, (state, { restaurants }) => ({
    ...state,
    restaurants,
    isLoading: false,
    error: null,
  })),

  on(AdminActions.loadUsersSuccess, (state, { users }) => ({
    ...state,
    users,
    isLoading: false,
    error: null,
  })),

  on(
    AdminActions.createCitySuccess,
    AdminActions.createRestaurantSuccess,
    AdminActions.deleteCitySuccess,
    AdminActions.deleteRestaurantSuccess,
    AdminActions.deleteUserSuccess,
    (state) => ({
      ...state,
      isLoading: false,
      error: null,
    })
  ),

  on(
    AdminActions.loadCitiesFailure,
    AdminActions.loadRestaurantsFailure,
    AdminActions.loadUsersFailure,
    AdminActions.createCityFailure,
    AdminActions.createRestaurantFailure,
    AdminActions.deleteCityFailure,
    AdminActions.deleteRestaurantFailure,
    AdminActions.deleteUserFailure,
    (state, { error }) => ({
      ...state,
      isLoading: false,
      error,
    })
  ),

  on(AdminActions.clearAdminError, (state) => ({
    ...state,
    error: null,
  })),

  on(AdminActions.clearAdminState, () => initialAdminState)
);