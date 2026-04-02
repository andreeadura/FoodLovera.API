import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface JoinSessionRequest {
  displayName: string;
}

export interface JoinSessionResponse {
  sessionId: number;
  participantId: number;
}

export interface SessionStatusResponse {
  sessionId: number;
  joinCode: string;
  requiredParticipants: number;
  currentParticipants: number;
}

export interface CreateSessionRequest {
  name: string;
  selectedCityId: number | null;
  useAllCategories: boolean;
  categoryIds: number[];
  latitude: number | null;
  longitude: number | null;
  requiredParticipants: number;
}

export interface CreateSessionResponse {
  sessionId: number;
  joinCode: string;
  name: string;
}

@Injectable({
  providedIn: 'root',
})
export class SessionApiService {
  private readonly http = inject(HttpClient);

  joinAsGuest(
    joinCode: string,
    request: JoinSessionRequest
  ): Observable<JoinSessionResponse> {
    return this.http.post<JoinSessionResponse>(
      `/api/sessions/${joinCode}/join`,
      request
    );
  }

  joinAsAuthenticated(joinCode: string): Observable<JoinSessionResponse> {
    return this.http.post<JoinSessionResponse>(
      `/api/sessions/${joinCode}/join/me`,
      {}
    );
  }

  getStatus(sessionId: number): Observable<SessionStatusResponse> {
    return this.http.get<SessionStatusResponse>(
      `/api/sessions/${sessionId}/status`
    );
  }

  createSession(
    request: CreateSessionRequest
  ): Observable<CreateSessionResponse> {
    return this.http.post<CreateSessionResponse>('/api/sessions', request);
  }
}