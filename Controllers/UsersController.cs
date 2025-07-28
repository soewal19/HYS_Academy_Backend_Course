using Microsoft.AspNetCore.Mvc;
using MeetingScheduler.Models;
using MeetingScheduler.Services;

namespace MeetingScheduler.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ScheduleService _scheduleService;

    public UsersController(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    // GET /users
    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _scheduleService.GetAllUsers();
        return Ok(users);
    }

    // GET /users/{userId}
    [HttpGet("{userId}")]
    public IActionResult GetUserById(int userId)
    {
        var user = _scheduleService.GetUserById(userId);
        return user == null ? NotFound() : Ok(user);
    }

    // POST /users
    [HttpPost]
    public IActionResult CreateUser([FromBody] UserCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required");

        var userId = _scheduleService.CreateUser(request.Name);
        return Ok(new { Id = userId });
    }

    // DELETE /users/{userId}
    [HttpDelete("{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        _scheduleService.DeleteUser(userId);
        return NoContent();
    }

    // GET /users/{userId}/meetings
    [HttpGet("{userId}/meetings")]
    public IActionResult GetUserMeetings(int userId)
    {
        var meetings = _scheduleService.GetUserMeetings(userId);
        return Ok(meetings);
    }
}

public class UserCreateRequest
{
    public string Name { get; set; }
}
