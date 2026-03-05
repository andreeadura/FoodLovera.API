import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { AuthResponse, LoginRequest, RegisterRequest } from '../../services/auth-api.service';

export const AuthActions = createActionGroup({
  source: 'Auth',
  events: {
    'Hydrate From Storage': emptyProps(),

    'Login Requested': props<{ request: LoginRequest }>(),
    'Register Requested': props<{ request: RegisterRequest }>(),

    'Auth Succeeded': props<{ response: AuthResponse }>(),
    'Auth Failed': props<{ error: string }>(),

    'Logout': emptyProps(),

    'Clear Error': emptyProps(),

    'Verify Email Requested': props<{ email: string; code: string }>(),
    'Resend Verification Requested': props<{ email: string }>(),

    'Set Pending Verification Email': props<{ email: string }>(),

    'Clear Verification Flow': emptyProps(),

    'Verify Email Succeeded': emptyProps(),
    'Verify Email Failed': props<{ error: string }>(),

  },
});
  