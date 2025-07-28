using System;
using System.Collections.Generic;

namespace MeetingScheduler.Models;

public class CreateMeetingRequest
{
    public List<int> ParticipantIds { get; set; } = new();
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
