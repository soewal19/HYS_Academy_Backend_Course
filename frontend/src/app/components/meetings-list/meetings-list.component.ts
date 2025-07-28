import { Component, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataProviderService } from '../../services/data-provider.service';
import { HttpClientModule } from '@angular/common/http';
import { Meeting, MeetingRequest } from '../../models/meeting.model';
import { User } from '../../models/user.model';
import { CalendarViewComponent } from '../calendar-view/calendar-view.component';
import { CalendarWeekViewComponent } from '../calendar-view/calendar-week-view.component';
import { generateICS } from '../../utils/ics-export';
import { parseICS } from '../../utils/ics-import';

import { OnInit } from '@angular/core';

@Component({
  selector: 'app-meetings-list',
  standalone: true,
  imports: [CommonModule, FormsModule, CalendarViewComponent, CalendarWeekViewComponent, HttpClientModule],
  templateUrl: './meetings-list.component.html',
  styleUrl: './meetings-list.component.css'
})
export class MeetingsListComponent implements OnInit {
  typingText = '';
  private typingFullText = 'Добро пожаловать в Meeting Scheduler!';
  private typingIndex = 0;
  private typingInterval: any;

  // Make Math available in template
  Math = Math;

  ngOnInit() {
    this.startTyping();
    this.loadUsers();
    this.loadMeetings();
    this.initializeNewMeeting();
  }

  loadUsers() {
    this.loading.set(true);
    this.api.getUsers().subscribe({
      next: (users) => {
        this.users.set(users);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Ошибка загрузки пользователей:', error);
        this.error.set('Не удалось загрузить пользователей');
        this.loading.set(false);
      }
    });
  }

  initializeNewMeeting() {
    const now = new Date();
    const later = new Date(now.getTime() + 60 * 60 * 1000); // +1 час
    
    this.newMeetingRequest.set({
      participantIds: [],
      durationMinutes: 30,
      earliestStart: now.toISOString().slice(0, 16),
      latestEnd: later.toISOString().slice(0, 16)
    });
  }
  startTyping() {
    this.typingText = '';
    this.typingIndex = 0;
    if (this.typingInterval) clearInterval(this.typingInterval);
    this.typingInterval = setInterval(() => {
      if (this.typingIndex < this.typingFullText.length) {
        this.typingText += this.typingFullText[this.typingIndex++];
      } else {
        clearInterval(this.typingInterval);
      }
    }, 70);
  }
  private api = inject(DataProviderService);

  // Основные данные
  meetings = signal<Meeting[]>([]);
  users = signal<User[]>([]);
  
  // Состояние загрузки и ошибок
  loading = signal<boolean>(false);
  error = signal<string | null>(null);
  notification = signal<string | null>(null);
  
  // Пагинация
  private readonly _itemsPerPage = signal<number>(5);
  currentPage = signal<number>(1);
  
  // Геттер и сеттер для itemsPerPage с валидацией
  get itemsPerPage(): number {
    return this._itemsPerPage();
  }
  
  set itemsPerPage(value: number) {
    if (value < 1) {
      console.warn('itemsPerPage must be at least 1');
      return;
    }
    this._itemsPerPage.set(value);
    // Сбрасываем на первую страницу при изменении количества элементов на странице
    this.currentPage.set(1);
  }

  // Вычисляемое свойство для отфильтрованных встреч с обработкой ошибок
  filteredMeetings = computed(() => {
    if (!this.meetings()) {
      return [];
    }
    let meetings = this.meetings();
    
    // Фильтрация по пользователю, если выбран
    if (this.userFilter()) {
      meetings = meetings.filter(m => m.participantIds.includes(this.userFilter()!));
    }
    
    // Фильтрация по дате, если задана
    if (this.dateFilterValue) {
      const filterDate = new Date(this.dateFilterValue).toDateString();
      meetings = meetings.filter(m => {
        const meetingDate = new Date(m.startTime).toDateString();
        return meetingDate === filterDate;
      });
    }
    
    // Фильтрация по группе пользователей, если задана
    if (this.groupUserIds().length) {
      meetings = meetings.filter(m => m.participantIds.some(pid => this.groupUserIds().includes(pid)));
    }
    
    // Фильтрация по длительности, если задана
    if (this.durationFilterValue) {
      meetings = meetings.filter(m => {
        const dur = (new Date(m.endTime).getTime() - new Date(m.startTime).getTime()) / 60000;
        return dur === +this.durationFilterValue;
      });
    }
    
    // Фильтрация по статусу, если задан
    if (this.statusFilterValue) {
      meetings = meetings.filter(m => m.status === this.statusFilterValue);
    }
    
    // Фильтрация по типу, если задан
    if (this.typeFilterValue) {
      meetings = meetings.filter(m => m.type === this.typeFilterValue);
    }
    
    return meetings;
  });

  // Вычисляемое свойство для общего количества элементов
  filteredMeetingsCount = computed(() => this.filteredMeetings().length);

  // Вычисляемое свойство для пагинированных данных
  paginatedMeetings = computed(() => {
    const startIndex = (this.currentPage() - 1) * this.itemsPerPage;
    return this.filteredMeetings().slice(startIndex, startIndex + this.itemsPerPage);
  });

  // Вычисляемое свойство для общего количества страниц
  totalPagesValue = computed(() => {
    return Math.ceil(this.filteredMeetings().length / this.itemsPerPage) || 1;
  });

  // Получение массива номеров страниц для пагинации
  pages = computed((): number[] => {
    const pages: number[] = [];
    const total = this.totalPagesValue();
    for (let i = 1; i <= total; i++) {
      pages.push(i);
    }
    return pages;
  });

  // Показывать ли многоточие перед страницей
  showPageEllipsis(page: number): boolean {
    const current = this.currentPage();
    const total = this.totalPagesValue(); // Вызываем как функцию
    
    // Показываем многоточие после первой страницы и перед последней
    if (page === 2 && current > 3) {
      return true;
    }
    
    // Показываем многоточие перед последней страницей
    if (page === total - 1 && current < total - 2) {
      return true;
    }
    
    return false;
  }
  
  // Нужно ли показывать страницу в пагинации
  shouldShowPage(page: number): boolean {
    const current = this.currentPage();
    const total = this.totalPagesValue(); // Вызываем как функцию
    
    // Всегда показываем первую и последнюю страницы
    if (page === 1 || page === total) {
      return true;
    }
    
    // Показываем текущую страницу и по одной странице до и после
    if (page >= current - 1 && page <= current + 1) {
      return true;
    }
    
    // Показываем вторую страницу, если текущая страница далеко от начала
    if (page === 2 && current > 3) {
      return false;
    }
    
    // Показываем предпоследнюю страницу, если текущая страница далеко от конца
    if (page === total - 1 && current < total - 2) {
      return false;
    }
    
    return false;
  }

  newMeetingRequest = signal<MeetingRequest>({
    participantIds: [],
    durationMinutes: 30,
    earliestStart: '',
    latestEnd: ''
  });
  creating = signal<boolean>(false);

  userFilter = signal<number | null>(null);
  dateFilter = signal<string>('');
  durationFilter = signal<string>('');
  statusFilter = signal<string>('');
  typeFilter = signal<string>('');
  groupUserIds = signal<number[]>([]);
  weekStart = signal<string>('');



  groupFreeSlots = computed(() => {
    const userIds = this.groupUserIds();
    const date = this.dateFilter();
    if (!date || !userIds.length) return [];
    const dayStart = new Date(date + 'T08:00:00');
    const dayEnd = new Date(date + 'T18:00:00');
    const meetingsByUser = userIds.map(uid =>
      this.meetings().filter(m => m.participantIds.includes(uid) && new Date(m.startTime).toISOString().slice(0, 10) === date)
    );
    const slots = [];
    let slotStart = new Date(dayStart);
    while (slotStart < dayEnd) {
      const slotEnd = new Date(slotStart.getTime() + 30 * 60000);
      const overlap = meetingsByUser.some(userMeetings =>
        userMeetings.some(m => {
          const mStart = new Date(m.startTime);
          const mEnd = new Date(m.endTime);
          return !(slotEnd <= mStart || slotStart >= mEnd);
        })
      );
      if (!overlap) slots.push({ start: new Date(slotStart), end: new Date(slotEnd) });
      slotStart = slotEnd;
    }
    return slots;
  });

  // --- Getters for template compatibility ---
  get meetingsList() { return this.meetings(); }
  get usersList() { return this.users(); }
  get isLoading() { return this.loading(); }
  get errorMsg() { return this.error(); }
  get notificationMsg() { return this.notification(); }
  get newMeeting() { return this.newMeetingRequest(); }
  get currentPageValue() { return this.currentPage(); }
  get isCreating() { return this.creating(); }
  get userFilterValue() { return this.userFilter(); }
  set userFilterValue(val: number | null) { this.userFilter.set(val); }
  get dateFilterValue() { return this.dateFilter(); }
  set dateFilterValue(val: string) { this.dateFilter.set(val); }
  get durationFilterValue() { return this.durationFilter(); }
  set durationFilterValue(val: string) { this.durationFilter.set(val); }
  get statusFilterValue() { return this.statusFilter(); }
  set statusFilterValue(val: string) { this.statusFilter.set(val); }
  get typeFilterValue() { return this.typeFilter(); }
  set typeFilterValue(val: string) { this.typeFilter.set(val); }
  get groupUserIdsValue() { return this.groupUserIds(); }
  set groupUserIdsValue(val: number[]) { this.groupUserIds.set(val); }
  get weekStartValue() { return this.weekStart(); }
  set weekStartValue(val: string) { this.weekStart.set(val); }
  // Метод для смены страницы с прокруткой вверх и валидацией
  setPage(page: number | string | Event): void {
    let pageNumber: number;
    
    // Обрабатываем событие изменения инпута
    if (page instanceof Event) {
      const target = page.target as HTMLInputElement;
      pageNumber = parseInt(target.value, 10);
    } else {
      pageNumber = typeof page === 'string' ? parseInt(page, 10) : page;
    }
    
    if (isNaN(pageNumber)) {
      console.warn('Invalid page number:', page);
      return;
    }
    
    const totalPages = this.totalPagesValue(); // Вызываем как функцию
    const newPage = Math.max(1, Math.min(pageNumber, totalPages));
    
    if (newPage !== this.currentPage()) {
      this.currentPage.set(newPage);
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }
  
  // Метод для перехода на следующую страницу
  nextPage(): void {
    this.setPage(this.currentPage() + 1);
  }
  
  // Метод для перехода на предыдущую страницу
  previousPage(): void {
    this.setPage(this.currentPage() - 1);
  }

  get groupFreeSlotsList() { return this.groupFreeSlots(); }

  // --- Helper for user name by id ---
  getUserNameById(id: number): string {
    return this.users().find(u => u.id === id)?.name || String(id);
  }

  // --- Popup state ---
  showCreateModal = false;
  showCreateUserModal = false;
  newUserName = '';

  selectedMeeting: Meeting | null = null;
  openMeetingDetails(meeting: Meeting) {
    this.selectedMeeting = meeting;
  }
  closeMeetingDetails() {
    this.selectedMeeting = null;
  }
  openCreateModal() { this.showCreateModal = true; }
  closeCreateModal() { this.showCreateModal = false; }
  openCreateUserModal() { this.showCreateUserModal = true; }
  closeCreateUserModal() { this.showCreateUserModal = false; }

  onCreateUserSubmit() {
    if (!this.newUserName.trim()) return;
    this.api.createUser(this.newUserName.trim()).subscribe({
      next: () => {
        this.loadUsers();
        this.newUserName = '';
        this.closeCreateUserModal();
      },
      error: () => {
        // обработка ошибки (можно добавить уведомление)
      }
    });
  }

  // --- Event handlers for calendar component ---
  onCreateMeetingFromSlot(event: any) {}
  onMoveMeetingToSlot(event: any) {}
  onDeleteMeeting(event: any) {}
  onEditMeeting(event: any) {}

  constructor() {
    this.loadMeetings();
    // Removed loadUsers() call here since it's already called in ngOnInit()
    const today = new Date();
    const monday = new Date(today);
    monday.setDate(today.getDate() - ((today.getDay() + 6) % 7));
    this.weekStart.set(monday.toISOString().slice(0, 10));
  }

  loadMeetings() {
    console.log('Loading meetings...');
    this.loading.set(true);
    this.error.set(null);
    
    this.api.getMeetings().subscribe({
      next: (data: Meeting[]) => {
        console.log('Successfully loaded meetings:', data);
        // Убедимся, что данные корректно обновляются в сигнале
        this.meetings.set([...data]);
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading meetings:', err);
        this.error.set('Не удалось загрузить список встреч. Пожалуйста, попробуйте обновить страницу.');
        this.loading.set(false);
      },
      complete: () => {
        console.log('Meetings loading completed');
      }
    });
  }

  createMeeting(onSuccess?: () => void) {
    const req = { ...this.newMeetingRequest() };
    // Преобразуем даты к ISO-строке, если они не пустые
    if (req.earliestStart) req.earliestStart = new Date(req.earliestStart).toISOString();
    if (req.latestEnd) req.latestEnd = new Date(req.latestEnd).toISOString();

    console.log('DEBUG req:', req);
    if (!req.participantIds.length || !req.earliestStart || !req.latestEnd || !req.durationMinutes) {
      this.error.set('Все поля обязательны для заполнения');
      return;
    }
    if (req.durationMinutes <= 0) {
      this.error.set('Длительность должна быть больше 0');
      return;
    }
    if (req.earliestStart >= req.latestEnd) {
      this.error.set('Время начала должно быть раньше времени окончания');
      return;
    }

    this.creating.set(true);
    this.error.set(null);

    console.log('Sending to backend:', req);
    this.api.createMeeting(req).subscribe({
      next: (createdMeeting) => {
        console.log('Meeting created:', createdMeeting);
        // Добавляем новую встречу в список и обновляем UI
        this.meetings.update(meetings => [...meetings, createdMeeting]);
        this.creating.set(false);
        this.newMeetingRequest.set({ 
          participantIds: [], 
          durationMinutes: 30, 
          earliestStart: '', 
          latestEnd: '' 
        });
        this.notification.set('Встреча успешно создана');
        setTimeout(() => this.notification.set(null), 3000);
        if (onSuccess) onSuccess();
        
        // Дополнительная синхронизация с бэкендом
        this.loadMeetings();
      },
      error: (err: any) => {
        console.error('Error creating meeting:', err);
        this.error.set(err.error?.message || 'Не удалось создать встречу');
        this.creating.set(false);
      }
    });
  }

  onCreateMeetingSubmit() {
    this.createMeeting(() => this.closeCreateModal());
  }

  deleteMeeting(id: number) {
    this.loading.set(true);
    this.error.set(null);
    this.api.deleteMeeting(id).subscribe({
      next: () => {
        this.meetings.update(list => list.filter(m => m.id !== id));
        this.loading.set(false);
        this.notification.set('Встреча удалена');
        setTimeout(() => this.notification.set(null), 2500);
      },
      error: (err: any) => {
        this.error.set('Не удалось удалить встречу');
        this.loading.set(false);
      }
    });
  }

  exportGroupSlotsToICS() {
    const slots = this.groupFreeSlots();
    if (!slots.length) return;
    const events = slots.map(slot => ({
      title: 'Free Group Slot',
      start: slot.start,
      end: slot.end,
      description: `Общий свободный слот для группы: ${this.groupUserIds().map(uid => this.users().find(u => u.id === uid)?.name).join(', ')}`
    }));
    this.downloadICS(events, 'group_free_slots.ics');
  }

  exportMeetingsToICS() {
    let meetingsToExport = this.filteredMeetings();
    const group = this.groupUserIds();
    if (group.length) {
      meetingsToExport = meetingsToExport.filter(m => m.participantIds.some(pid => group.includes(pid)));
    }
    if (!meetingsToExport.length) return;
    const events = meetingsToExport.map(m => ({
      title: `Meeting #${m.id}`,
      start: new Date(m.startTime),
      end: new Date(m.endTime),
      description: `Участники: ${m.participantIds.map(pid => this.users().find(u => u.id === pid)?.name || pid).join(', ')}`
    }));
    this.downloadICS(events, 'meetings.ics');
  }

  private downloadICS(events: any[], filename: string) {
    const ics = generateICS(events);
    const blob = new Blob([ics], { type: 'text/calendar' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    setTimeout(() => URL.revokeObjectURL(url), 2000);
  }

  importICS(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    const file = input.files[0];
    const reader = new FileReader();
    reader.onload = () => {
      const text = reader.result as string;
      const events = parseICS(text);
      for (const ev of events) {
        const participantIds = this.groupUserIds().length ? [...this.groupUserIds()] : this.users().map(u => u.id);
        const duration = Math.round((ev.end.getTime() - ev.start.getTime()) / 60000);
        this.newMeetingRequest.set({
          participantIds,
          durationMinutes: duration,
          earliestStart: ev.start.toISOString().slice(0, 16),
          latestEnd: ev.end.toISOString().slice(0, 16),
        });
        this.createMeeting();
      }
    };
    reader.readAsText(file);
    input.value = '';
  }

  syncWithGoogle() {
    this.notification.set('Синхронизация с Google Calendar находится в разработке.');
    setTimeout(() => this.notification.set(null), 3000);
  }

  syncWithOutlook() {
    this.notification.set('Синхронизация с Outlook находится в разработке.');
    setTimeout(() => this.notification.set(null), 3000);
  }
}
