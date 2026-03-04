import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError, map, shareReplay } from 'rxjs/operators';

export type ImageConfig = Record<string, string>;

@Injectable({ providedIn: 'root' })
export class ImageConfigService {
  private readonly cache = new Map<string, Observable<ImageConfig>>();

  constructor(private readonly http: HttpClient) {}


  load(page: string): Observable<ImageConfig> {
    const key = page.trim().toLowerCase();
    const cached = this.cache.get(key);
    if (cached) return cached;

    const request$ = this.http
      .get<ImageConfig>(`/assets/imageConfig/${key}.json`)
      .pipe(
        catchError(() => of({} as ImageConfig)),
        shareReplay(1)
      );

    this.cache.set(key, request$);
    return request$;
  }


  get(config: ImageConfig | null | undefined, assetKey: string): string {
    if (!config) return '';
    return config[assetKey] ?? '';
  }

  get$(page: string, assetKey: string): Observable<string> {
    return this.load(page).pipe(map(cfg => this.get(cfg, assetKey)));
  }
}