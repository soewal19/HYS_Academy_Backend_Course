import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Meeting, MeetingRequest } from '../models/meeting.model';
import { User } from '../models/user.model';
import { ApiService } from './api.service';
import { MockApiService } from './mock-api.service';
import { DATA_PROVIDER_MODE, DataProviderMode } from './data-provider.token';

@Injectable({ providedIn: 'root' })
export class DataProviderService {
  constructor(
    private backend: ApiService,
    private mock: MockApiService,
    @Inject(DATA_PROVIDER_MODE) private mode: DataProviderMode
  ) {}

  getMeetings(): Observable<Meeting[]> {
    return this.mode === 'mock' ? this.mock.getMeetings() : this.backend.getMeetings();
  }
  getUsers(): Observable<User[]> {
    return this.mode === 'mock' ? this.mock.getUsers() : this.backend.getUsers();
  }
  createMeeting(req: MeetingRequest): Observable<Meeting> {
    return this.mode === 'mock' ? this.mock.createMeeting(req) : this.backend.createMeeting(req);
  }
  deleteMeeting(id: number): Observable<void> {
    return this.mode === 'mock' ? this.mock.deleteMeeting(id) : this.backend.deleteMeeting(id);
  }

  findAvailableSlots(request: MeetingRequest): Observable<string[]> {
    return this.mode === 'mock' ? this.mock.findAvailableSlots(request) : this.backend.findAvailableSlots(request);
  }

  createUser(name: string): Observable<any> {
    return this.mode === 'mock'
      ? this.mock.createUser(name)
      : this.backend.createUser(name);
  }
}
