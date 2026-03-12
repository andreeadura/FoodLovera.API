import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import * as AdminActions from '../store/actions/admin.actions';
import {
  selectAdminError,
  selectAdminLoading,
  selectAdminUsers,
  selectCities,
  selectRestaurants,
} from '../store/selectors/admin.selectors';

type AdminTab = 'cities' | 'restaurants' | 'users';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminDashboardComponent implements OnInit {
  private readonly store = inject(Store);
  private readonly fb = inject(FormBuilder);
  private readonly translate = inject(TranslateService);

  readonly cities$ = this.store.select(selectCities);
  readonly restaurants$ = this.store.select(selectRestaurants);
  readonly users$ = this.store.select(selectAdminUsers);
  readonly loading$ = this.store.select(selectAdminLoading);
  readonly error$ = this.store.select(selectAdminError);

  tab: AdminTab = 'cities';

  readonly cityForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
  });

  readonly restaurantForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    cityId: [0, [Validators.required, Validators.min(1)]],
    imageUrl: ['', [Validators.required, Validators.maxLength(1000)]],
    isActive: [true],
  });

  ngOnInit(): void {
    this.store.dispatch(AdminActions.loadCities());
    this.store.dispatch(AdminActions.loadRestaurants());
    this.store.dispatch(AdminActions.loadUsers());
  }

  switchTo(tab: AdminTab): void {
    this.tab = tab;
    this.store.dispatch(AdminActions.clearAdminError());
  }

  submitCity(): void {
    if (this.cityForm.invalid) {
      this.cityForm.markAllAsTouched();
      return;
    }

    const request = this.cityForm.getRawValue();

    this.store.dispatch(
      AdminActions.createCity({
        request: { name: request.name.trim() },
      })
    );

    this.cityForm.reset({ name: '' });
  }

  submitRestaurant(): void {
    if (this.restaurantForm.invalid) {
      this.restaurantForm.markAllAsTouched();
      return;
    }

    const request = this.restaurantForm.getRawValue();

    this.store.dispatch(
      AdminActions.createRestaurant({
        request: {
          name: request.name.trim(),
          cityId: Number(request.cityId),
          imageUrl: request.imageUrl.trim(),
          isActive: request.isActive,
        },
      })
    );

    this.restaurantForm.reset({
      name: '',
      cityId: 0,
      imageUrl: '',
      isActive: true,
    });
  }

  deleteCity(cityId: number, cityName: string): void {
    const message = this.translate.instant('admin.confirm.deleteCity', {
      name: cityName,
    });

    if (!window.confirm(message)) {
      return;
    }

    this.store.dispatch(AdminActions.deleteCity({ cityId }));
  }

  deleteRestaurant(restaurantId: number, restaurantName: string): void {
    const message = this.translate.instant('admin.confirm.deleteRestaurant', {
      name: restaurantName,
    });

    if (!window.confirm(message)) {
      return;
    }

    this.store.dispatch(AdminActions.deleteRestaurant({ restaurantId }));
  }

  deleteUser(userId: number, email: string): void {
    const message = this.translate.instant('admin.confirm.deleteUser', {
      email,
    });

    if (!window.confirm(message)) {
      return;
    }

    this.store.dispatch(AdminActions.deleteUser({ userId }));
  }

  trackById(_: number, item: { id: number }): number {
    return item.id;
  }
}