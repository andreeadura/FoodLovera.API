import { ApplicationConfig, isDevMode, importProvidersFrom, APP_INITIALIZER } from '@angular/core';
import { provideRouter } from '@angular/router';
import { HttpClient, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { routes } from './app.routes';

import { provideStore, provideState } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';

import { authFeatureKey, authReducer } from './modules/auth/store/reducer/auth.reducer';
import { AuthEffects } from './modules/auth/store/effects/auth.effects';

import { sessionsFeatureKey, sessionsReducer } from './modules/sessions/store/reducer/sessions.reducer';
import { SessionsEffects } from './modules/sessions/store/effects/sessions.effects';

import { adminFeatureKey, adminReducer } from './modules/admin/store/reducer/admin.reducer';
import { AdminEffects } from './modules/admin/store/effects/admin.effects';

import { userFeatureKey, userReducer } from './modules/user/store/reducer/user.reducer';
import { UserEffects } from './modules/user/store/effects/user.effects';

import { Observable, firstValueFrom } from 'rxjs';

import {
  TranslateLoader,
  TranslateModule,
  TranslationObject,
  TranslateService
} from '@ngx-translate/core';

import { withInterceptors } from '@angular/common/http';
import { authTokenInterceptor } from './modules/auth/interceptors/auth-token.interceptor';

class SimpleJsonTranslateLoader implements TranslateLoader {
  constructor(
    private readonly http: HttpClient,
    private readonly prefix = '/assets/translations/',
    private readonly suffix = '.json'
  ) {}

  getTranslation(lang: string): Observable<TranslationObject> {
    return this.http.get<TranslationObject>(`${this.prefix}${lang}${this.suffix}`);
  }
}

export function translateLoaderFactory(http: HttpClient): TranslateLoader {
  return new SimpleJsonTranslateLoader(http);
}



export function initTranslationsFactory(translate: TranslateService) {
  return async () => {
    translate.setDefaultLang('en');
    await firstValueFrom(translate.use('en'));
  };
}


export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),

    provideHttpClient(
      withInterceptorsFromDi(),
      withInterceptors([authTokenInterceptor])
    ),

  
    importProvidersFrom(
      TranslateModule.forRoot({
        defaultLanguage: 'ro',
        useDefaultLang: true,
        loader: {
          provide: TranslateLoader,
          useFactory: translateLoaderFactory,
          deps: [HttpClient],
        },
      })
    ),

    {
      provide: APP_INITIALIZER,
      useFactory: initTranslationsFactory,
      deps: [TranslateService],
      multi: true,
    },

   

    provideStore(),

    provideState(authFeatureKey, authReducer),
    provideState(sessionsFeatureKey, sessionsReducer),
    provideState(adminFeatureKey, adminReducer),
    provideState(userFeatureKey, userReducer),

    provideEffects(
      AuthEffects,
      SessionsEffects,
      AdminEffects,
      UserEffects
    ),

    

    provideStoreDevtools({
      maxAge: 25,
      logOnly: !isDevMode(),
    }),
  ],
};