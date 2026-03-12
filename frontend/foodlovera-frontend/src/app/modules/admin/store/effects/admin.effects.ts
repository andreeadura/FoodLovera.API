import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, of } from 'rxjs';

import { AdminApiService } from '../../services/admin-api.service';
import * as AdminActions from '../actions/admin.actions';

@Injectable()
export class AdminEffects {
  private readonly actions$ = inject(Actions);
  private readonly api = inject(AdminApiService);

  loadCities$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdminActions.loadCities),
      mergeMap(() =>
        this.api.loadCities().pipe(
          map((cities) => AdminActions.loadCitiesSuccess({ cities })),
          catchError((err) =>
            of(AdminActions.loadCitiesFailure({ error: this.extractError(err) }))
          )
        )
      )
    )
  );

  createCity$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdminActions.createCity),
      mergeMap(({ request }) =>
        this.api.createCity(request).pipe(
          map(() => AdminActions.createCitySuccess()),
          catchError((err) =>
            of(AdminActions.createCityFailure({ error: this.extractError(err) }))
          )
        )
      )
    )
  );

  reloadCitiesAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdminActions.createCitySuccess, AdminActions.deleteCitySuccess),
      map(() => AdminActions.loadCities())
    )
  );

  deleteCity$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdminActions.deleteCity),
      mergeMap(({ cityId }) =>
        this.api.deleteCity(cityId).pipe(
          map(() => AdminActions.deleteCitySuccess()),
          catchError((err) =>
            of(AdminActions.deleteCityFailure({ error: this.extractError(err) }))
          )
        )
      )
    )
  );

  loadRestaurants$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdminActions.loadRestaurants),
      mergeMap(() =>
        this.api.loadRestaurants().pipe(
          map((restaurants) => AdminActions.loadRestaurantsSuccess({ restaurants })),
          catchError((err) =>
            of(
              AdminActions.loadRestaurantsFailure({
                error: this.extractError(err),
              })
            )
          )
        )
      )
    )
  );

  createRestaurant$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdminActions.createRestaurant),
      mergeMap(({ request }) =>
        this.api.createRestaurant(request).pipe(
          map(() => AdminActions.createRestaurantSuccess()),
          catchError((err) =>
            of(
              AdminActions.createRestaurantFailure({
                error: this.extractError(err),
              })
            )
          )
        )
      )
    )
  );

  reloadRestaurantsAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        AdminActions.createRestaurantSuccess,
        AdminActions.deleteRestaurantSuccess
      ),
      map(() => AdminActions.loadRestaurants())
    )
  );

  deleteRestaurant$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdminActions.deleteRestaurant),
      mergeMap(({ restaurantId }) =>
        this.api.deleteRestaurant(restaurantId).pipe(
          map(() => AdminActions.deleteRestaurantSuccess()),
          catchError((err) =>
            of(
              AdminActions.deleteRestaurantFailure({
                error: this.extractError(err),
              })
            )
          )
        )
      )
    )
  );

  loadUsers$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdminActions.loadUsers),
      mergeMap(() =>
        this.api.loadUsers().pipe(
          map((users) => AdminActions.loadUsersSuccess({ users })),
          catchError((err) =>
            of(AdminActions.loadUsersFailure({ error: this.extractError(err) }))
          )
        )
      )
    )
  );

  deleteUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdminActions.deleteUser),
      mergeMap(({ userId }) =>
        this.api.deleteUser(userId).pipe(
          map(() => AdminActions.deleteUserSuccess()),
          catchError((err) =>
            of(AdminActions.deleteUserFailure({ error: this.extractError(err) }))
          )
        )
      )
    )
  );

  reloadUsersAfterDelete$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdminActions.deleteUserSuccess),
      map(() => AdminActions.loadUsers())
    )
  );

  private extractError(err: unknown): string {
    const error = err as {
      error?: { detail?: string; title?: string };
      message?: string;
    };

    return (
      error?.error?.detail ??
      error?.error?.title ??
      error?.message ??
      'A apărut o eroare.'
    );
  }
}