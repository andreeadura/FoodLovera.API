import { createAction, props } from '@ngrx/store';
import {
  AdminCity,
  AdminRestaurant,
  AdminUser,
  CreateCityRequest,
  CreateRestaurantRequest,
} from '../../services/admin-api.service';

// Cities
export const loadCities = createAction('[Admin] Load Cities');
export const loadCitiesSuccess = createAction(
  '[Admin] Load Cities Success',
  props<{ cities: AdminCity[] }>()
);
export const loadCitiesFailure = createAction(
  '[Admin] Load Cities Failure',
  props<{ error: string }>()
);

export const createCity = createAction(
  '[Admin] Create City',
  props<{ request: CreateCityRequest }>()
);
export const createCitySuccess = createAction('[Admin] Create City Success');
export const createCityFailure = createAction(
  '[Admin] Create City Failure',
  props<{ error: string }>()
);

export const deleteCity = createAction(
  '[Admin] Delete City',
  props<{ cityId: number }>()
);
export const deleteCitySuccess = createAction('[Admin] Delete City Success');
export const deleteCityFailure = createAction(
  '[Admin] Delete City Failure',
  props<{ error: string }>()
);

// Restaurants
export const loadRestaurants = createAction('[Admin] Load Restaurants');
export const loadRestaurantsSuccess = createAction(
  '[Admin] Load Restaurants Success',
  props<{ restaurants: AdminRestaurant[] }>()
);
export const loadRestaurantsFailure = createAction(
  '[Admin] Load Restaurants Failure',
  props<{ error: string }>()
);

export const createRestaurant = createAction(
  '[Admin] Create Restaurant',
  props<{ request: CreateRestaurantRequest }>()
);
export const createRestaurantSuccess = createAction('[Admin] Create Restaurant Success');
export const createRestaurantFailure = createAction(
  '[Admin] Create Restaurant Failure',
  props<{ error: string }>()
);

export const deleteRestaurant = createAction(
  '[Admin] Delete Restaurant',
  props<{ restaurantId: number }>()
);
export const deleteRestaurantSuccess = createAction('[Admin] Delete Restaurant Success');
export const deleteRestaurantFailure = createAction(
  '[Admin] Delete Restaurant Failure',
  props<{ error: string }>()
);

// Users
export const loadUsers = createAction('[Admin] Load Users');
export const loadUsersSuccess = createAction(
  '[Admin] Load Users Success',
  props<{ users: AdminUser[] }>()
);
export const loadUsersFailure = createAction(
  '[Admin] Load Users Failure',
  props<{ error: string }>()
);

export const deleteUser = createAction(
  '[Admin] Delete User',
  props<{ userId: number }>()
);
export const deleteUserSuccess = createAction('[Admin] Delete User Success');
export const deleteUserFailure = createAction(
  '[Admin] Delete User Failure',
  props<{ error: string }>()
);

export const clearAdminError = createAction('[Admin] Clear Error');
export const clearAdminState = createAction('[Admin] Clear State');