using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MeetingScheduler.Models;
using MeetingScheduler.Services;

namespace MeetingScheduler.Controllers;

[ApiController]
[Route("meetings")]
public class MeetingsController : ControllerBase
{
    private readonly ScheduleService _scheduleService;
    private readonly ILogger<MeetingsController> _logger;

    public MeetingsController(ScheduleService scheduleService, ILogger<MeetingsController> logger)
    {
        _scheduleService = scheduleService;
        _logger = logger;
    }

    [HttpPost]
    public IActionResult CreateMeeting([FromBody] CreateMeetingRequest request)
    {
        if (request == null)
        {
            return BadRequest("Meeting request is required");
        }

        if (request.ParticipantIds == null || request.ParticipantIds.Count == 0)
        {
            return BadRequest("At least one participant is required");
        }

        if (request.Start >= request.End)
        {
            return BadRequest("End time must be after start time");
        }

        try
        {
            // Создаем MeetingRequest на основе CreateMeetingRequest
            var meetingRequest = new MeetingRequest
            {
                ParticipantIds = request.ParticipantIds,
                EarliestStart = request.Start,
                LatestEnd = request.End,
                DurationMinutes = (int)(request.End - request.Start).TotalMinutes
            };
            
            var meeting = _scheduleService.ScheduleMeeting(meetingRequest);
            
            if (meeting == null)
            {
                return BadRequest("Could not schedule meeting. No available time slots or invalid participants.");
            }
            
            return CreatedAtAction(nameof(GetMeetingById), new { id = meeting.Id }, meeting);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating meeting");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetMeetingById(int id)
    {
        try
        {
            var meeting = _scheduleService.GetMeetingById(id);
            if (meeting == null)
            {
                return NotFound();
            }
            return Ok(meeting);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting meeting with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("user/{userId}")]
    public IActionResult GetMeetingsForUser(int userId)
    {
        try
        {
            var meetings = _scheduleService.GetMeetingsForUser(userId);
            return Ok(meetings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting meetings for user {userId}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMeeting(int id)
    {
        try
        {
            var meeting = _scheduleService.GetMeetingById(id);
            if (meeting == null)
            {
                return NotFound();
            }
            
            // Удаляем встречу с использованием существующего метода DeleteMeeting
            _scheduleService.DeleteMeeting(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting meeting with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }
}
