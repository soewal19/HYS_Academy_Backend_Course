import { Meeting } from './meeting.model';

export const MOCK_MEETINGS: Meeting[] = [
  {
    id: 1,
    participantIds: [1, 2],
    startTime: '2025-07-21T10:00:00.000Z',
    endTime: '2025-07-21T10:30:00.000Z',
    status: 'active',
    type: 'online'
  },
  {
    id: 2,
    participantIds: [2, 3],
    startTime: '2025-07-22T11:00:00.000Z',
    endTime: '2025-07-22T12:00:00.000Z',
    status: 'done',
    type: 'offline'
  },
  {
    id: 3,
    participantIds: [1, 3],
    startTime: '2025-07-23T09:30:00.000Z',
    endTime: '2025-07-23T10:30:00.000Z',
    status: 'cancelled',
    type: 'group'
  },
  {
    id: 4,
    participantIds: [1, 2, 3],
    startTime: '2025-07-24T14:00:00.000Z',
    endTime: '2025-07-24T15:00:00.000Z',
    status: 'active',
    type: 'group'
  },
  {
    id: 5,
    participantIds: [2],
    startTime: '2025-07-25T16:00:00.000Z',
    endTime: '2025-07-25T16:30:00.000Z',
    status: 'active',
    type: 'online'
  }
];
