using System;
using System.Collections.Generic;

namespace MeetingScheduler.Models;

public class MeetingRequest
{
    public List<int> ParticipantIds { get; set; } = new();
    public int DurationMinutes { get; set; }
    public DateTime EarliestStart { get; set; }
    public DateTime LatestEnd { get; set; }
}
