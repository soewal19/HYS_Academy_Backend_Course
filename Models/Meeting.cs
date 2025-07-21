public class Meeting
{
    public int Id { get; set; }
    public List<int> ParticipantIds { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}