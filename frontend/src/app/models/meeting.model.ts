export interface Meeting {
  id: number;
  participantIds: number[];
  startTime: string; // ISO string
  endTime: string;   // ISO string
}

export interface MeetingRequest {
  participantIds: number[];
  durationMinutes: number;
  earliestStart: string; // ISO string
  latestEnd: string;     // ISO string
} 