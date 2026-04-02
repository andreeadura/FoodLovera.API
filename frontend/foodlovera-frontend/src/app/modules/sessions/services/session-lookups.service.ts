import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface SessionCityLookup {
  id: number;
  name: string;
}

export interface SessionCategoryLookup {
  id: number;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class SessionLookupsService {
  private readonly http = inject(HttpClient);

  loadCities(): Observable<SessionCityLookup[]> {
    return this.http.get<SessionCityLookup[]>('/api/lookups/cities');
  }

  loadCategories(): Observable<SessionCategoryLookup[]> {
    return this.http.get<SessionCategoryLookup[]>('/api/lookups/categories');
  }
}