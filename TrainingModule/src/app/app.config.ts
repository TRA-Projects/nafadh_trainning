import { ApplicationConfig, ErrorHandler, LOCALE_ID, inject, isDevMode, provideAppInitializer, provideZoneChangeDetection } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { provideState, provideStore } from '@ngrx/store';
import { provideStoreDevtools } from '@ngrx/store-devtools';

import { readPersistedLocale } from './core/locale/locale-persistence';
import { routes } from './app.routes';
import { JobseekerProfileApiService } from './core/api/services/jobseeker-profile-api.service';
import { JOBSEEKER_PROFILE_ACTIONS } from './core/profile/jobseeker-profile-actions.token';
import { ClientErrorReporter } from './core/errors/client-error-reporter.service';
import { GlobalErrorHandler } from './core/errors/global-error-handler';
import { authBearerInterceptor } from './core/interceptors/auth-bearer.interceptor';
import { apiBaseUrlInterceptor } from './core/interceptors/api-base-url.interceptor';
import { httpErrorInterceptor } from './core/interceptors/http-error.interceptor';
import { localeFeature } from './core/store/locale/locale.reducer';
import { jobseekerProfileFeature } from './core/store/jobseeker-profile/jobseeker-profile.reducer';
import { uiAsyncFeature } from './core/store/ui-async/ui-async.reducer';
import { adminSystemFeature } from './core/store/admin-system/admin-system.reducer';
import { registrationFeature } from './core/store/registration/registration.reducer';
import { identityAccessFeature } from './core/store/identity-access/identity-access.reducer';
import { notificationsFeature } from './core/store/notifications/notifications.reducer';
import { showcaseFeature } from './core/store/showcase/showcase.reducer';
import { AuthService } from './core/auth/auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideAppInitializer(() => {
      inject(AuthService).restoreSessionFromTokens();
      inject(ClientErrorReporter).installGlobalHandlers();
    }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([apiBaseUrlInterceptor, authBearerInterceptor, httpErrorInterceptor])),
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    { provide: LOCALE_ID, useFactory: (): string => readPersistedLocale() },
    { provide: JOBSEEKER_PROFILE_ACTIONS, useExisting: JobseekerProfileApiService },
    provideStore(),
    provideState(localeFeature),
    provideState(jobseekerProfileFeature),
    provideState(uiAsyncFeature),
    provideState(adminSystemFeature),
    provideState(registrationFeature),
    provideState(identityAccessFeature),
    provideState(notificationsFeature),
    provideState(showcaseFeature),
    ...(isDevMode()
      ? [
          provideStoreDevtools({
            maxAge: 25,
            trace: false,
            connectInZone: true,
          }),
        ]
      : []),
  ],
};
