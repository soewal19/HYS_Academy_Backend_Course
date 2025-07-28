import { InjectionToken } from '@angular/core';

export type DataProviderMode = 'backend' | 'mock';

export const DATA_PROVIDER_MODE = new InjectionToken<DataProviderMode>('DATA_PROVIDER_MODE');
