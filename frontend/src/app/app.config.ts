import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';

import { DATA_PROVIDER_MODE } from './services/data-provider.token';
import { environment } from '../environments/environment';
import { AbstractApiService } from './services/abstract-api.service';
import { ApiService } from './services/api.service';
import { MockApiService } from './services/mock-api.service';

const useMock = true; // ВСЕГДА mock-режим для демонстрации

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideClientHydration(),
    provideHttpClient(),
    {
      provide: AbstractApiService,
      useClass: environment.useMockApi ? MockApiService : ApiService
    },
    { provide: DATA_PROVIDER_MODE, useValue: useMock ? 'mock' : 'backend' }
  ]
};
