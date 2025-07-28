import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Meeting, MeetingRequest } from '../models/meeting.model';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export abstract class AbstractApiService {
  abstract getMeetings(): Observable<Meeting[]>;
  abstract createMeeting(request: MeetingRequest): Observable<Meeting>;
  abstract deleteMeeting(id: number): Observable<void>;
  abstract findAvailableSlots(request: MeetingRequest): Observable<string[]>;

  abstract getUsers(): Observable<User[]>;
}
