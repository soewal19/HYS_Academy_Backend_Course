import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user.model';
import { Meeting, MeetingRequest } from '../models/meeting.model';
import { AbstractApiService } from './abstract-api.service';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService extends AbstractApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { 
    super();
  }

  // Users
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/users`);
  }

  createUser(name: string): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${this.apiUrl}/users`, { name });
  }

  // Meetings
  getMeetings(): Observable<Meeting[]> {
    return this.http.get<Meeting[]>(`${this.apiUrl}/meetings`);
  }

  createMeeting(request: MeetingRequest): Observable<Meeting> {
    return this.http.post<Meeting>(`${this.apiUrl}/meetings`, request);
  }

  deleteMeeting(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/meetings/${id}`);
  }

  findAvailableSlots(request: MeetingRequest): Observable<string[]> {
    return this.http.post<string[]>(`${this.apiUrl}/meetings/available-slots`, request);
  }
}