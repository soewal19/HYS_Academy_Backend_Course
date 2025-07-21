using Microsoft.AspNetCore.Mvc;
using MeetingScheduler.Models;
using MeetingScheduler.Services;

namespace MeetingScheduler.Controllers;

[ApiController]
[Route("[controller]")]
public class MeetingsController : ControllerBase
{
    private readonly ScheduleService _scheduleService;

    public MeetingsController(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpPost]
    public IActionResult CreateMeeting([FromBody] MeetingRequest request)
    {
        if (!request.IsValid())
        {
            return BadRequest("Invalid meeting request");
        }

        var meeting = _scheduleService.ScheduleMeeting(request);
        
        if (meeting == null)
        {
            return NotFound("No available time slot found");
        }

        return Ok(meeting);
    }

    [HttpGet("users/{userId}")]
    public IActionResult GetUserMeetings(int userId)
    {
        var meetings = _scheduleService.GetUserMeetings(userId);
        return Ok(meetings);
    }
}

