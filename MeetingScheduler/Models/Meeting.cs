namespace MeetingScheduler.Models;

public class Meeting
{
    public int Id { get; set; }
    public List<int> ParticipantIds { get; set; } = new();
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    // For compatibility with tests
    public DateTime StartTime { get => Start; set => Start = value; }
    public DateTime EndTime { get => End; set => End = value; }
}
