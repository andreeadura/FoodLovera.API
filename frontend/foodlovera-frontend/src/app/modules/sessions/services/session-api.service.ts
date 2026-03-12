import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface JoinSessionRequest {
  displayName: string;
}

export interface JoinSessionResponse {
  sessionId: number;
  participantId: number;
}

@Injectable({ providedIn: 'root' })
export class SessionApiService {
  private readonly http = inject(HttpClient);

  joinAsGuest(joinCode: string, request: JoinSessionRequest): Observable<JoinSessionResponse> {
    return this.http.post<JoinSessionResponse>(`/api/sessions/${joinCode}/join`, request);
  }

  joinAsAuthenticated(joinCode: string): Observable<JoinSessionResponse> {
    return this.http.post<JoinSessionResponse>(`/api/sessions/${joinCode}/join/me`, {});
  }
}