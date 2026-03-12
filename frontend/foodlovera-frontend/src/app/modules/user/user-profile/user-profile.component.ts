import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { TranslateModule } from '@ngx-translate/core';
import { Subject, takeUntil } from 'rxjs';

import { AuthActions } from '../../auth/store/actions/auth.actions';
import * as UserActions from '../store/actions/user.actions';
import {
  selectPasswordChanged,
  selectUserError,
  selectUserLoading,
  selectUserProfile,
  selectUsernameUpdated,
} from '../store/selectors/user.selectors';

type ProfileSection = 'details' | 'security';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserProfileComponent implements OnInit, OnDestroy {
  private readonly store = inject(Store);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly destroy$ = new Subject<void>();

  readonly profile$ = this.store.select(selectUserProfile);
  readonly loading$ = this.store.select(selectUserLoading);
  readonly error$ = this.store.select(selectUserError);
  readonly usernameUpdated$ = this.store.select(selectUsernameUpdated);
  readonly passwordChanged$ = this.store.select(selectPasswordChanged);

  section: ProfileSection = 'details';
  showPasswordForm = false;

  readonly detailsForm = this.fb.nonNullable.group({
    username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
    email: [{ value: '', disabled: true }],
  });

  readonly passwordForm = this.fb.nonNullable.group({
    oldPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
  });

  ngOnInit(): void {
    this.store.dispatch(UserActions.loadProfile());

    this.profile$
      .pipe(takeUntil(this.destroy$))
      .subscribe((profile) => {
        if (!profile) {
          this.detailsForm.reset({
            username: '',
            email: '',
          });
          return;
        }

        this.detailsForm.patchValue({
          username: profile.username ?? '',
          email: profile.email,
        });
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  openDetails(): void {
    this.section = 'details';
    this.showPasswordForm = false;
    this.store.dispatch(UserActions.clearUserError());
    this.store.dispatch(UserActions.clearUserStatus());
  }

  openSecurity(): void {
    this.section = 'security';
    this.showPasswordForm = false;
    this.store.dispatch(UserActions.clearUserError());
    this.store.dispatch(UserActions.clearUserStatus());
  }

  openChangePassword(): void {
    this.section = 'security';
    this.showPasswordForm = true;
    this.store.dispatch(UserActions.clearUserError());
    this.store.dispatch(UserActions.clearUserStatus());
  }

  saveUsername(): void {
    if (this.detailsForm.invalid) {
      this.detailsForm.markAllAsTouched();
      return;
    }

    const value = this.detailsForm.getRawValue();

    this.store.dispatch(
      UserActions.updateUsername({
        request: {
          username: value.username.trim(),
        },
      })
    );
  }

  submitPasswordChange(): void {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    const value = this.passwordForm.getRawValue();

    this.store.dispatch(
      UserActions.changePassword({
        request: {
          currentPassword: value.oldPassword,
          newPassword: value.newPassword,
        },
      })
    );

    this.passwordForm.reset({
      oldPassword: '',
      newPassword: '',
    });
  }

  goToForgotPassword(): void {
    this.router.navigate([{ outlets: { modal: ['auth', 'login'] } }], {
      queryParams: { forgotPassword: '1' },
    });
  }

  logout(): void {
    this.store.dispatch(UserActions.clearUserState());
    this.store.dispatch(AuthActions.logout());
  }
}