import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AdminCity {
  id: number;
  name: string;
}

export interface AdminRestaurant {
  id: number;
  name: string;
  cityId: number;
  cityName: string;
  imageUrl: string;
  isActive: boolean;
  createdAt: string;
}

export interface AdminUser {
  id: number;
  email: string;
  username: string | null;
  isEmailVerified: boolean;
  role: string;
  createdAt: string;
}

export interface CreateCityRequest {
  name: string;
}

export interface CreateRestaurantRequest {
  name: string;
  cityId: number;
  imageUrl: string;
  isActive: boolean;
}

export interface CreatedResponse {
  id: number;
}

@Injectable({ providedIn: 'root' })
export class AdminApiService {
  private readonly citiesUrl = '/api/admin/cities';
  private readonly restaurantsUrl = '/api/admin/restaurants';
  private readonly usersUrl = '/api/admin/users';

  constructor(private readonly http: HttpClient) {}

  loadCities(): Observable<AdminCity[]> {
    return this.http.get<AdminCity[]>(this.citiesUrl);
  }

  createCity(request: CreateCityRequest): Observable<CreatedResponse> {
    return this.http.post<CreatedResponse>(this.citiesUrl, request);
  }

  deleteCity(cityId: number): Observable<void> {
    return this.http.delete<void>(`${this.citiesUrl}/${cityId}`);
  }

  loadRestaurants(): Observable<AdminRestaurant[]> {
    return this.http.get<AdminRestaurant[]>(this.restaurantsUrl);
  }

  createRestaurant(request: CreateRestaurantRequest): Observable<CreatedResponse> {
    return this.http.post<CreatedResponse>(this.restaurantsUrl, request);
  }

  deleteRestaurant(restaurantId: number): Observable<void> {
    return this.http.delete<void>(`${this.restaurantsUrl}/${restaurantId}`);
  }

  loadUsers(): Observable<AdminUser[]> {
    return this.http.get<AdminUser[]>(this.usersUrl);
  }

  deleteUser(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.usersUrl}/${userId}`);
  }
}