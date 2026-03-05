import { createAction, props } from '@ngrx/store';

// Cities
export const loadCities = createAction('[Admin] Load Cities');
export const loadCitiesSuccess = createAction(
  '[Admin] Load Cities Success',
  props<{ cities: any[] }>()
);
export const loadCitiesFailure = createAction(
  '[Admin] Load Cities Failure',
  props<{ error: string }>()
);

// Restaurants
export const loadRestaurants = createAction('[Admin] Load Restaurants');
export const loadRestaurantsSuccess = createAction(
  '[Admin] Load Restaurants Success',
  props<{ restaurants: any[] }>()
);
export const loadRestaurantsFailure = createAction(
  '[Admin] Load Restaurants Failure',
  props<{ error: string }>()
);

// Users
export const loadUsers = createAction('[Admin] Load Users');
export const loadUsersSuccess = createAction(
  '[Admin] Load Users Success',
  props<{ users: any[] }>()
);
export const loadUsersFailure = createAction(
  '[Admin] Load Users Failure',
  props<{ error: string }>()
);

// Clear
export const clearAdminState = createAction('[Admin] Clear State');