import { ChangeDetectionStrategy, Component, HostListener, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { Store } from '@ngrx/store';

import { AuthActions } from '../store/actions/auth.actions';
import { selectAuthError, selectAuthLoading ,selectNeedsEmailVerification,selectPendingVerificationEmail,selectVerificationSuccess} from '../store/selectors/auth.selectors';


type Tab = 'login' | 'register';

@Component({
  selector: 'app-auth-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './auth-modal.component.html',
  styleUrls: ['./auth-modal.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AuthModalComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly store = inject(Store);

  readonly loading$ = this.store.select(selectAuthLoading);
  readonly needsEmailVerification$ = this.store.select(selectNeedsEmailVerification);
  readonly pendingEmail$ = this.store.select(selectPendingVerificationEmail);
  readonly verificationSuccess$ = this.store.select(selectVerificationSuccess);
  readonly error$ = this.store.select(selectAuthError) as unknown as import('rxjs').Observable<string | null>;

  tab: Tab = 'login';



readonly verifyForm = this.fb.group({
  code: ['', [Validators.required]]
});

readonly loginForm = this.fb.group({
  email: ['', [Validators.required, Validators.email]],
  password: ['', [Validators.required]]
});

readonly registerForm = this.fb.group({
  username: ['', [Validators.required]],
  email: ['', [Validators.required, Validators.email]],
  password: ['', [Validators.required]]
});

  close(): void {
    this.store.dispatch(AuthActions.clearError());
    this.router.navigate([{ outlets: { modal: null } }]);
    this.store.dispatch(AuthActions.clearVerificationFlow());
    
  }

  @HostListener('document:keydown.escape')
  onEsc(): void {
    this.close();
  }

  switchTo(tab: Tab): void {
    this.tab = tab;
    this.store.dispatch(AuthActions.clearError());
  }

  submitLogin(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const request = this.loginForm.getRawValue() as { email: string; password: string };
    this.store.dispatch(AuthActions.loginRequested({ request }));
  }

  submitRegister(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const request = this.registerForm.getRawValue() as { username: string; email: string; password: string };
    this.store.dispatch(AuthActions.registerRequested({ request }));
  }
    ngOnInit(): void {
    this.store.dispatch(AuthActions.clearError());
  }

  submitVerify(email: string): void {
  if (this.verifyForm.invalid) {
    this.verifyForm.markAllAsTouched();
    return;
  }

  const code = (this.verifyForm.value.code ?? '').trim();
  this.store.dispatch(AuthActions.verifyEmailRequested({ email, code }));
  }

  resend(email: string): void {
  this.store.dispatch(AuthActions.resendVerificationRequested({ email }));
  }

  backToRegister(): void {
  this.store.dispatch(AuthActions.clearError());
  this.verifyForm.reset();
  this.tab = 'register';
  
}

goToLogin(): void {
  this.verifyForm.reset();
  this.tab = 'login';
  this.store.dispatch(AuthActions.clearError());
  this.store.dispatch(AuthActions.clearVerificationFlow());
}
}

