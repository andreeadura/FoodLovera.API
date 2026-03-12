import { inject } from '@angular/core';
import { CanMatchFn, Router, UrlTree } from '@angular/router';
import { AuthTokenStorage } from '../services/auth-token.storage';

function decodeJwtPayload(token: string): Record<string, unknown> | null {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) {
      return null;
    }

    const payload = parts[1]
      .replace(/-/g, '+')
      .replace(/_/g, '/');

    const json = decodeURIComponent(
      atob(payload)
        .split('')
        .map((c) => `%${(`00${c.charCodeAt(0).toString(16)}`).slice(-2)}`)
        .join('')
    );

    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return null;
  }
}

function getRoleClaims(payload: Record<string, unknown>): string[] {
  const possibleRoleKeys = [
    'role',
    'roles',
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
  ];

  const values: string[] = [];

  for (const key of possibleRoleKeys) {
    const rawValue = payload[key];

    if (typeof rawValue === 'string') {
      values.push(rawValue);
    }

    if (Array.isArray(rawValue)) {
      for (const item of rawValue) {
        if (typeof item === 'string') {
          values.push(item);
        }
      }
    }
  }

  return values;
}

function isAdminRole(role: string): boolean {
  const normalized = role.trim().toLowerCase();
  return normalized === 'admin' || normalized === '1';
}

export const authGuard: CanMatchFn = (): boolean | UrlTree => {
  const tokenStorage = inject(AuthTokenStorage);
  const router = inject(Router);

  const token = tokenStorage.get();

  if (!token) {
    return router.parseUrl('/');
  }

  const payload = decodeJwtPayload(token);

  if (!payload) {
    tokenStorage.clear();
    return router.parseUrl('/');
  }

  const exp = payload['exp'];
  if (typeof exp === 'number') {
    const nowInSeconds = Math.floor(Date.now() / 1000);
    if (exp <= nowInSeconds) {
      tokenStorage.clear();
      return router.parseUrl('/');
    }
  }

  return true;
};

export const adminGuard: CanMatchFn = (): boolean | UrlTree => {
  const tokenStorage = inject(AuthTokenStorage);
  const router = inject(Router);

  const token = tokenStorage.get();

  if (!token) {
    return router.parseUrl('/');
  }

  const payload = decodeJwtPayload(token);

  if (!payload) {
    tokenStorage.clear();
    return router.parseUrl('/');
  }

  const exp = payload['exp'];
  if (typeof exp === 'number') {
    const nowInSeconds = Math.floor(Date.now() / 1000);
    if (exp <= nowInSeconds) {
      tokenStorage.clear();
      return router.parseUrl('/');
    }
  }

  const roles = getRoleClaims(payload);
  const isAdmin = roles.some(isAdminRole);

  if (!isAdmin) {
    return router.parseUrl('/');
  }

  return true;
};