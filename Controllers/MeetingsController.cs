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

    [HttpGet]
    public IActionResult GetMeetings()
    {
        var meetings = _scheduleService.GetAllMeetings();
        return Ok(meetings);
    }

    [HttpGet("{id}")]
    public IActionResult GetMeetingById(int id)
    {
        var meeting = _scheduleService.GetMeetingById(id);
        return meeting == null ? NotFound() : Ok(meeting);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMeeting(int id)
    {
        _scheduleService.DeleteMeeting(id);
        return NoContent();
    }

    [HttpGet("users/{userId}")]
    public IActionResult GetUserMeetings(int userId)
    {
        var meetings = _scheduleService.GetUserMeetings(userId);
        return Ok(meetings);
    }
}

