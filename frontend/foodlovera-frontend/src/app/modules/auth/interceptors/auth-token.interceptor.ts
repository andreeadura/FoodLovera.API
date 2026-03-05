import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthTokenStorage } from '../services/auth-token.storage';

export const authTokenInterceptor: HttpInterceptorFn = (req, next) => {
  const storage = inject(AuthTokenStorage);
  const token = storage.get();

  if (!token || req.url.startsWith('/assets/')) {
    return next(req);
  }

  return next(
    req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    })
  );
};