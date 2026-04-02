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

export interface NextRequest {
  participantId: number;
}

export interface LikeRequest {
  participantId: number;
}

export interface WinnerResponse {
  restaurantId: number;
  restaurantName: string;
  reason: number;
}

export interface GameRestaurant {
  id: number;
  name: string;
  imageUrl: string;
  categories: string[];
}

export interface GameStateResponse {
  completed: boolean;
  isUnanimousMatch: boolean;
  serverUtcNow: string;
  roundEndsAtUtc: string | null;
  roundNumber: number;
  currentRestaurant: GameRestaurant | null;
  myVoteIsLike: boolean | null;
  winners: WinnerResponse[];
}

export interface SetVoteRequest {
  participantId: number;
  isLike: boolean;
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

  getGameState(
    sessionId: number,
    participantId: number
  ): Observable<GameStateResponse> {
    return this.http.get<GameStateResponse>(
      `/api/sessions/${sessionId}/game-state`,
      {
        params: {
          participantId,
        },
      }
    );
  }

  setVote(
    sessionId: number,
    request: SetVoteRequest
  ): Observable<GameStateResponse> {
    return this.http.put<GameStateResponse>(
      `/api/sessions/${sessionId}/vote`,
      request
    );
  }
}