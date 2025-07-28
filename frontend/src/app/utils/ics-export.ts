// Утилита для экспорта встреч/слотов в формат .ics (iCalendar)
export function generateICS(events: { title: string; start: Date; end: Date; description?: string }[]): string {
  const pad = (n: number) => n.toString().padStart(2, '0');
  const formatDate = (d: Date) =>
    `${d.getUTCFullYear()}${pad(d.getUTCMonth() + 1)}${pad(d.getUTCDate())}T${pad(d.getUTCHours())}${pad(d.getUTCMinutes())}${pad(d.getUTCSeconds())}Z`;
  const lines = [
    'BEGIN:VCALENDAR',
    'VERSION:2.0',
    'PRODID:-//MeetingScheduler//RU',
  ];
  for (const ev of events) {
    lines.push('BEGIN:VEVENT');
    lines.push(`SUMMARY:${ev.title}`);
    lines.push(`DTSTART:${formatDate(ev.start)}`);
    lines.push(`DTEND:${formatDate(ev.end)}`);
    if (ev.description) lines.push(`DESCRIPTION:${ev.description}`);
    lines.push('END:VEVENT');
  }
  lines.push('END:VCALENDAR');
  return lines.join('\r\n');
}
