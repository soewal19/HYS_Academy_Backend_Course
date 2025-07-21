import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user.model';
import { Meeting, MeetingRequest } from '../models/meeting.model';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private apiUrl = 'http://localhost:5000';

  constructor(private http: HttpClient) {}

  // Users
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/users`);
  }

  createUser(name: string): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(`${this.apiUrl}/users`, { name });
  }

  // Meetings
  getUserMeetings(userId: number): Observable<Meeting[]> {
    return this.http.get<Meeting[]>(`${this.apiUrl}/meetings/users/${userId}`);
  }

  createMeeting(request: MeetingRequest): Observable<Meeting> {
    return this.http.post<Meeting>(`${this.apiUrl}/meetings`, request);
  }

  getAllMeetings(): Observable<Meeting[]> {
    // (если потребуется)
    return this.http.get<Meeting[]>(`${this.apiUrl}/meetings`);
  }
} 