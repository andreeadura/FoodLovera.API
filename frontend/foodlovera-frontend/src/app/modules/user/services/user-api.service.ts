import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserProfile {
  email: string;
  username: string | null;
}

export interface UpdateUsernameRequest {
  username: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

@Injectable({ providedIn: 'root' })
export class UserApiService {
  private readonly usersUrl = '/api/users';
  private readonly authUrl = '/api/auth';

  constructor(private readonly http: HttpClient) {}

  getMyProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.usersUrl}/me`);
  }

  updateUsername(request: UpdateUsernameRequest): Observable<void> {
    return this.http.put<void>(`${this.usersUrl}/me/username`, request);
  }

  changePassword(request: ChangePasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.authUrl}/change-password`, request);
  }
}