import { Routes } from '@angular/router';

export const SESSION_ROUTES: Routes = [
  {
    path: ':sessionId',
    loadComponent: () =>
      import('./session-waiting/session-waiting.component').then(
        (m) => m.SessionWaitingComponent
      ),
  },
  {
    path: ':sessionId/next',
    loadComponent: () =>
      import('./session-next/session-next.component').then(
        (m) => m.SessionNextComponent
      ),
  },
];