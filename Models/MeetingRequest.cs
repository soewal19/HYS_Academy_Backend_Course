public class MeetingRequest
{
    [Required, MinLength(1)]
    public List<int> ParticipantIds { get; set; }
    
    [Required, Range(1, 480)] // 8 часов в минутах
    public int DurationMinutes { get; set; }
    
    [Required]
    public DateTime EarliestStart { get; set; }
    
    [Required]
    public DateTime LatestEnd { get; set; }
    
    public bool IsValid() => EarliestStart < LatestEnd && 
                           DurationMinutes <= (LatestEnd - EarliestStart).TotalMinutes;
}