import { CommonModule } from '@angular/common';
import { Component, Input, signal, computed } from '@angular/core';
import { Meeting } from '../../models/meeting.model';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-calendar-view',
  standalone: true,
  templateUrl: './calendar-view.component.html',
  styleUrl: './calendar-view.component.css',
  imports: [CommonModule]
})
export class CalendarViewComponent {
  @Input() meetings: Meeting[] = [];
  @Input() users: User[] = [];
  @Input() date: string = '';
  @Input() userFilter: number | null = null;
  @Input() groupSlots: { start: Date; end: Date }[] = [];

  // Вычисляем встречи на выбранный день и для выбранного пользователя
  get dayMeetingsList() {
    if (!this.date) return [];
    const date = new Date(this.date);
    return this.meetings.filter(m => {
      const mStart = new Date(m.startTime);
      return (
        mStart.getFullYear() === date.getFullYear() &&
        mStart.getMonth() === date.getMonth() &&
        mStart.getDate() === date.getDate() &&
        (!this.userFilter || m.participantIds.includes(this.userFilter))
      );
    });
  }

  getUserNameById(id: number): string {
    return this.users.find(u => u.id === id)?.name || String(id);
  }
}
