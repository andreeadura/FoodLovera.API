import {
  ChangeDetectionStrategy,
  Component,
  HostListener,
  OnInit,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { forkJoin } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import {
  CreateSessionResponse,
  SessionApiService,
} from '../services/session-api.service';
import {
  SessionCategoryLookup,
  SessionCityLookup,
  SessionLookupsService,
} from '../services/session-lookups.service';

@Component({
  selector: 'app-create-room',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './create-room.component.html',
  styleUrls: ['./create-room.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateRoomComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly sessionApi = inject(SessionApiService);
  private readonly lookupsApi = inject(SessionLookupsService);
  private readonly translate = inject(TranslateService);

  cities: SessionCityLookup[] = [];
  categories: SessionCategoryLookup[] = [];

  isLoadingLookups = true;
  isSubmitting = false;
  serverErrorKey: string | null = null;
  locationErrorKey: string | null = null;

  isCategoriesOpen = false;

  readonly currentLocationOptionValue = 'current';

  readonly form = this.fb.group({
    name: this.fb.nonNullable.control('', [
      Validators.required,
      Validators.minLength(2),
      Validators.maxLength(100),
    ]),
    selectedCityId: this.fb.control<number | null>(null),
    useCurrentLocation: this.fb.nonNullable.control(false),
    selectedCategoryIds: this.fb.nonNullable.control<number[]>([]),
    useAllCategories: this.fb.nonNullable.control(true),
    requiredParticipants: this.fb.nonNullable.control(2, [
      Validators.required,
      Validators.min(1),
      Validators.max(20),
    ]),
    latitude: this.fb.control<number | null>(null),
    longitude: this.fb.control<number | null>(null),
  });

  ngOnInit(): void {
    this.loadLookups();
  }

  get nameControl(): AbstractControl {
    return this.form.controls.name;
  }

  get requiredParticipantsControl(): AbstractControl {
    return this.form.controls.requiredParticipants;
  }

  get citySelectValue(): string {
    if (this.form.controls.useCurrentLocation.value) {
      return this.currentLocationOptionValue;
    }

    const selectedCityId = this.form.controls.selectedCityId.value;
    return selectedCityId != null ? String(selectedCityId) : '';
  }

  get categoriesTriggerLabel(): string {
    if (this.form.controls.useAllCategories.value) {
      return this.translate.instant('createRoom.selectAllCategories');
    }

    const selectedIds = this.form.controls.selectedCategoryIds.value;
    const selectedNames = this.categories
      .filter((category) => selectedIds.includes(category.id))
      .map((category) => category.name);

    if (selectedNames.length === 0) {
      return this.translate.instant('createRoom.categoriesPlaceholder');
    }

    if (selectedNames.length <= 2) {
      return selectedNames.join(', ');
    }

    return this.translate.instant('createRoom.categoriesSelected', {
      count: selectedNames.length,
    });
  }

  private loadLookups(): void {
    this.isLoadingLookups = true;

    forkJoin({
      cities: this.lookupsApi.loadCities(),
      categories: this.lookupsApi.loadCategories(),
    })
      .pipe(finalize(() => (this.isLoadingLookups = false)))
      .subscribe({
        next: ({ cities, categories }) => {
          this.cities = cities;
          this.categories = categories;
        },
        error: () => {
          this.serverErrorKey = 'createRoom.errors.lookupsFailed';
        },
      });
  }

  onCityChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.locationErrorKey = null;

    if (!value) {
      this.form.patchValue({
        selectedCityId: null,
        useCurrentLocation: false,
        latitude: null,
        longitude: null,
      });
      return;
    }

    if (value === this.currentLocationOptionValue) {
      this.form.patchValue({
        selectedCityId: null,
        useCurrentLocation: true,
      });

      this.requestCurrentLocation();
      return;
    }

    this.form.patchValue({
      selectedCityId: Number(value),
      useCurrentLocation: false,
      latitude: null,
      longitude: null,
    });
  }

  toggleCategoriesDropdown(event: MouseEvent): void {
    event.stopPropagation();

    if (this.isLoadingLookups) {
      return;
    }

    this.isCategoriesOpen = !this.isCategoriesOpen;
  }

  onCategoriesPanelClick(event: MouseEvent): void {
    event.stopPropagation();
  }

  selectAllCategories(event: MouseEvent): void {
    event.stopPropagation();

    this.form.patchValue({
      useAllCategories: true,
      selectedCategoryIds: [],
    });
  }

  toggleCategory(categoryId: number, event: MouseEvent): void {
    event.stopPropagation();

    const selectedIds = [...this.form.controls.selectedCategoryIds.value];
    const index = selectedIds.indexOf(categoryId);

    if (index >= 0) {
      selectedIds.splice(index, 1);
    } else {
      selectedIds.push(categoryId);
    }

    this.form.patchValue({
      useAllCategories: selectedIds.length === 0,
      selectedCategoryIds: selectedIds,
    });
  }

  isCategorySelected(categoryId: number): boolean {
    return this.form.controls.selectedCategoryIds.value.includes(categoryId);
  }

  private requestCurrentLocation(): void {
    if (!navigator.geolocation) {
      this.locationErrorKey = 'createRoom.errors.geolocationUnavailable';
      this.form.patchValue({
        useCurrentLocation: false,
        latitude: null,
        longitude: null,
      });
      return;
    }

    navigator.geolocation.getCurrentPosition(
      (position) => {
        this.form.patchValue({
          latitude: position.coords.latitude,
          longitude: position.coords.longitude,
        });
        this.locationErrorKey = null;
      },
      () => {
        this.form.patchValue({
          useCurrentLocation: false,
          latitude: null,
          longitude: null,
        });
        this.locationErrorKey = 'createRoom.errors.locationDenied';
      },
      {
        enableHighAccuracy: true,
        timeout: 10000,
        maximumAge: 0,
      }
    );
  }

  submit(): void {
    this.serverErrorKey = null;

    if (
      !this.form.controls.useCurrentLocation.value &&
      this.form.controls.selectedCityId.value == null
    ) {
      this.serverErrorKey = 'createRoom.errors.cityRequired';
      return;
    }

    if (
      this.form.controls.useCurrentLocation.value &&
      (this.form.controls.latitude.value == null ||
        this.form.controls.longitude.value == null)
    ) {
      this.serverErrorKey = 'createRoom.errors.locationRequired';
      return;
    }

    if (
      !this.form.controls.useAllCategories.value &&
      this.form.controls.selectedCategoryIds.value.length === 0
    ) {
      this.serverErrorKey = 'createRoom.errors.categoriesRequired';
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const request = {
      name: this.form.controls.name.value.trim(),
      selectedCityId: this.form.controls.useCurrentLocation.value
        ? null
        : this.form.controls.selectedCityId.value,
      useAllCategories: this.form.controls.useAllCategories.value,
      categoryIds: this.form.controls.useAllCategories.value
        ? []
        : this.form.controls.selectedCategoryIds.value,
      latitude: this.form.controls.useCurrentLocation.value
        ? this.form.controls.latitude.value
        : null,
      longitude: this.form.controls.useCurrentLocation.value
        ? this.form.controls.longitude.value
        : null,
      requiredParticipants: this.form.controls.requiredParticipants.value,
    };

    this.isSubmitting = true;

    this.sessionApi
      .createSession(request)
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: (response: CreateSessionResponse) => {
          this.router.navigate(['/create-room/success'], {
            queryParams: {
              sessionId: response.sessionId,
              joinCode: response.joinCode,
              name: response.name,
            },
          });
        },
        error: (err: HttpErrorResponse) => {
          const payload = err.error as
            | {
                messageKey?: string;
                detailKey?: string;
                message?: string;
                detail?: string;
              }
            | null
            | undefined;

          this.serverErrorKey =
            payload?.messageKey ??
            payload?.detailKey ??
            payload?.message ??
            payload?.detail ??
            'createRoom.errors.generic';
        },
      });
  }

  @HostListener('document:click')
  onDocumentClick(): void {
    this.isCategoriesOpen = false;
  }
}