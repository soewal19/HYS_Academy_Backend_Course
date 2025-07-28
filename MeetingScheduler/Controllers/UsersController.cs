using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MeetingScheduler.Models;
using MeetingScheduler.Services;

namespace MeetingScheduler.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly ScheduleService _scheduleService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ScheduleService scheduleService, ILogger<UsersController> logger)
    {
        _scheduleService = scheduleService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        try
        {
            // Возвращаем список пользователей из сервиса
            return Ok(_scheduleService.GetAllUsers());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        try
        {
            var user = _scheduleService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting user with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("User name is required");
        }

        try
        {
            var userId = _scheduleService.CreateUser(request.Name);
            var user = new User { Id = userId, Name = request.Name };
            return CreatedAtAction(nameof(GetUserById), new { id = userId }, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        try
        {
            var result = _scheduleService.DeleteUser(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user with id {id}");
            return StatusCode(500, "Internal server error");
        }
    }
}
