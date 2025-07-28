// Утилита для чтения .ics (iCalendar) и парсинга событий (минимальный парсер)
export interface ParsedICSEvent {
  title: string;
  start: Date;
  end: Date;
  description?: string;
}

export function parseICS(ics: string): ParsedICSEvent[] {
  const events: ParsedICSEvent[] = [];
  const eventBlocks = ics.split('BEGIN:VEVENT').slice(1);
  for (const block of eventBlocks) {
    const lines = block.split(/\r?\n/);
    let title = '', dtstart = '', dtend = '', desc = '';
    for (const line of lines) {
      if (line.startsWith('SUMMARY:')) title = line.replace('SUMMARY:', '').trim();
      if (line.startsWith('DTSTART:')) dtstart = line.replace('DTSTART:', '').trim();
      if (line.startsWith('DTEND:')) dtend = line.replace('DTEND:', '').trim();
      if (line.startsWith('DESCRIPTION:')) desc = line.replace('DESCRIPTION:', '').trim();
    }
    if (dtstart && dtend) {
      events.push({
        title,
        start: icsDateToJS(dtstart),
        end: icsDateToJS(dtend),
        description: desc || undefined,
      });
    }
  }
  return events;
}

function icsDateToJS(dt: string): Date {
  // Формат: 20250726T090000Z
  const y = +dt.slice(0, 4);
  const m = +dt.slice(4, 6) - 1;
  const d = +dt.slice(6, 8);
  const h = +dt.slice(9, 11);
  const min = +dt.slice(11, 13);
  const s = +dt.slice(13, 15);
  return new Date(Date.UTC(y, m, d, h, min, s));
}
