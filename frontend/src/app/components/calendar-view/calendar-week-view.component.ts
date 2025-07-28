import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Meeting } from '../../models/meeting.model';
import { User } from '../../models/user.model';

interface Slot {
  start: Date;
  end: Date;
  isGroup?: boolean;
}

@Component({
  selector: 'app-calendar-week-view',
  standalone: true,
  templateUrl: './calendar-week-view.component.html',
  styleUrl: './calendar-week-view.component.css',
  imports: [CommonModule]
})
export class CalendarWeekViewComponent {
  @Input() meetings: Meeting[] = [];
  @Input() users: User[] = [];
  @Input() weekStart: string = '';
  @Input() userFilter: number | null = null;
  @Input() groupSlots: Slot[] = [];
  @Output() createMeetingFromSlot = new EventEmitter<Slot>();
  @Output() moveMeetingToSlot = new EventEmitter<{ meetingId: number, slot: Slot }>();
  @Output() deleteMeeting = new EventEmitter<Meeting>();
  @Output() editMeeting = new EventEmitter<Meeting>();

  onMeetingDragStart(meeting: Meeting, event: DragEvent) {
    event.dataTransfer?.setData('meetingId', meeting.id.toString());
  }

  onSlotDrop(slot: Slot, event: DragEvent) {
    const meetingId = Number(event.dataTransfer?.getData('meetingId'));
    if (meetingId) {
      this.moveMeetingToSlot.emit({ meetingId, slot });
    }
  }

  onDeleteMeeting(meeting: Meeting, event: Event) {
    event.stopPropagation();
    this.deleteMeeting.emit(meeting);
  }

  onEditMeeting(meeting: Meeting, event: Event) {
    event.stopPropagation();
    this.editMeeting.emit(meeting);
  }

  hasConflict(meeting: Meeting): boolean {
    // Проверить, есть ли у кого-то из участников другие встречи, пересекающиеся по времени
    const ms = new Date(meeting.startTime).getTime();
    const me = new Date(meeting.endTime).getTime();
    return meeting.participantIds.some(pid =>
      this.meetings.some(m2 =>
        m2.id !== meeting.id &&
        m2.participantIds.includes(pid) &&
        ((new Date(m2.startTime).getTime() < me) && (new Date(m2.endTime).getTime() > ms))
      )
    );
  }

  getMeetingTooltip(meeting: Meeting): string {
    const users = meeting.participantIds.map(pid => this.getUserNameById(pid)).join(', ');
    const start = new Date(meeting.startTime).toLocaleString();
    const end = new Date(meeting.endTime).toLocaleString();
    return `Участники: ${users}\nВремя: ${start} — ${end}`;
  }

  getUserNameById(id: number): string {
    return this.users.find(u => u.id === id)?.name || String(id);
  }

  // Генерируем массив дней недели начиная с weekStart
  get weekDays(): Date[] {
    const start = this.weekStart ? new Date(this.weekStart) : new Date();
    const days: Date[] = [];
    for (let i = 0; i < 5; i++) {
      const d = new Date(start);
      d.setDate(start.getDate() + i);
      days.push(d);
    }
    return days;
  }

  // Получить встречи на конкретный день
  meetingsForDay(date: Date): Meeting[] {
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

  // Получить групповые слоты на конкретный день
  groupSlotsForDay(date: Date): Slot[] {
    return this.groupSlots.filter(slot => {
      return (
        slot.start.getFullYear() === date.getFullYear() &&
        slot.start.getMonth() === date.getMonth() &&
        slot.start.getDate() === date.getDate()
      );
    });
  }

  onGroupSlotClick(slot: Slot) {
    this.createMeetingFromSlot.emit(slot);
  }
}
