import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Meeting, MeetingRequest } from '../models/meeting.model';
import { User } from '../models/user.model';
import { MOCK_MEETINGS } from '../models/mock-meetings';
const INITIAL_MOCK_MEETINGS = [
  {
    id: 1,
    participantIds: [1, 2],
    startTime: '2025-07-21T10:00:00.000Z',
    endTime: '2025-07-21T10:30:00.000Z',
    status: 'active' as 'active',
    type: 'online' as 'online'
  },
  {
    id: 2,
    participantIds: [2, 3],
    startTime: '2025-07-22T11:00:00.000Z',
    endTime: '2025-07-22T12:00:00.000Z',
    status: 'done' as 'done',
    type: 'offline' as 'offline'
  },
  {
    id: 3,
    participantIds: [1, 3],
    startTime: '2025-07-23T09:30:00.000Z',
    endTime: '2025-07-23T10:30:00.000Z',
    status: 'cancelled' as 'cancelled',
    type: 'group' as 'group'
  }
];
import { AbstractApiService } from './abstract-api.service';

@Injectable({
  providedIn: 'root'
})
export class MockApiService extends AbstractApiService {

  private mockUsers: User[] = [
    { id: 1, name: 'Alice' },
    { id: 2, name: 'Bob' },
    { id: 3, name: 'Charlie' }
  ];

  constructor() { 
    super();
  }

  getMeetings(): Observable<Meeting[]> {
    // Не сбрасывать моковые встречи при каждом запросе!
    console.log('MOCK_MEETINGS:', MOCK_MEETINGS);
    return of(MOCK_MEETINGS);
  }

  createMeeting(request: MeetingRequest): Observable<Meeting> {
    const meeting: Meeting = {
      id: Math.max(0, ...MOCK_MEETINGS.map(m => m.id)) + 1,
      participantIds: request.participantIds,
      startTime: request.earliestStart,
      endTime: request.latestEnd,
      status: 'active',
      type: 'online',
    };
    MOCK_MEETINGS.push(meeting);
    console.log('Created meeting:', meeting, 'All meetings:', MOCK_MEETINGS);
    return of(meeting);
  }

  deleteMeeting(id: number): Observable<void> {
    const index = MOCK_MEETINGS.findIndex(m => m.id === id);
    if (index > -1) {
      MOCK_MEETINGS.splice(index, 1);
    }
    return of(undefined);
  }

  getUsers(): Observable<User[]> {
    console.log('MOCK USERS:', this.mockUsers);
    return of(this.mockUsers);
  }

  createUser(name: string): Observable<{ id: number }> {
    const id = Math.max(0, ...this.mockUsers.map(u => u.id)) + 1;
    this.mockUsers.push({ id, name });
    return of({ id });
  }

  findAvailableSlots(request: MeetingRequest): Observable<string[]> {
    // Mock implementation: return a single slot for simplicity
    const slot = new Date(request.earliestStart);
    slot.setHours(10, 0, 0, 0); // Always return 10:00 AM
    return of([slot.toISOString()]);
  }
}
