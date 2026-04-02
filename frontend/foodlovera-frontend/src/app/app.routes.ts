import { Routes } from '@angular/router';
import { AppShellComponent } from './layout/app-shell/app-shell.component';
import { AuthModalComponent } from './modules/auth/auth-modal/auth-modal.component';
import { adminGuard, authGuard } from './modules/auth/guards/admin.guards';

export const routes: Routes = [
  {
    path: 'auth',
    outlet: 'modal',
    children: [
      { path: 'login', component: AuthModalComponent },
      { path: 'register', component: AuthModalComponent },
    ],
  },
  {
    path: '',
    component: AppShellComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./modules/homepage/homepage.component').then((m) => m.HomepageComponent),
      },
      {
        path: 'join-room',
        loadComponent: () =>
          import('./modules/sessions/join-room/join-room.component').then(
            (m) => m.JoinRoomComponent
          ),
      },
      {
        path: 'create-room',
        loadComponent: () =>
          import('./modules/sessions/create-room/create-room.component').then(
            (m) => m.CreateRoomComponent
          ),
      },
      {
        path: 'create-room/success',
        loadComponent: () =>
          import('./modules/sessions/create-room-success/create-room-success.component').then(
            (m) => m.CreateRoomSuccessComponent
          ),
      },
      {
        path: 'sessions',
        loadChildren: () =>
          import('./modules/sessions/sessions.routes').then((m) => m.SESSION_ROUTES),
      },
      {
        path: 'user',
        canMatch: [authGuard],
        loadChildren: () =>
          import('./modules/user/user.module').then((m) => m.UserModule),
      },
      {
        path: 'admin',
        canMatch: [adminGuard],
        loadChildren: () =>
          import('./modules/admin/admin.module').then((m) => m.AdminModule),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];