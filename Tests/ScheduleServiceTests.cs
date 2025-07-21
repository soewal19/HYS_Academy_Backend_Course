using MeetingScheduler.Models;
using MeetingScheduler.Services;
using Xunit;

namespace MeetingScheduler.Tests;

public class ScheduleServiceTests
{
    private readonly ScheduleService _service;

    public ScheduleServiceTests()
    {
        _service = new ScheduleService();
    }

    [Fact]
    public void CreateUser_ShouldReturnUserId()
    {
        // Act
        var userId = _service.CreateUser("Alice");

        // Assert
        Assert.Equal(1, userId);
    }

    [Fact]
    public void ScheduleMeeting_WithAvailableTimeSlot_ShouldCreateMeeting()
    {
        // Arrange
        _service.CreateUser("Alice");
        _service.CreateUser("Bob");
        
        var request = new MeetingRequest
        {
            ParticipantIds = new List<int> { 1, 2 },
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var meeting = _service.ScheduleMeeting(request);

        // Assert
        Assert.NotNull(meeting);
        Assert.Equal(1, meeting.Id);
        Assert.Equal(2, meeting.ParticipantIds.Count);
        Assert.Equal(request.EarliestStart, meeting.StartTime);
        Assert.Equal(request.EarliestStart.AddMinutes(60), meeting.EndTime);
    }

    [Fact]
    public void ScheduleMeeting_WithConflictingMeetings_ShouldFindNextAvailableSlot()
    {
        // Arrange
        _service.CreateUser("Alice");
        _service.CreateUser("Bob");
        
        // Создаем конфликтующую встречу
        var existingMeeting = new Meeting
        {
            Id = 1,
            ParticipantIds = new List<int> { 1 },
            StartTime = new DateTime(2025, 6, 20, 10, 0, 0, DateTimeKind.Utc),
            EndTime = new DateTime(2025, 6, 20, 11, 0, 0, DateTimeKind.Utc)
        };
        _service.AddMeeting(existingMeeting);

        var request = new MeetingRequest
        {
            ParticipantIds = new List<int> { 1 },
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var meeting = _service.ScheduleMeeting(request);

        // Assert
        Assert.NotNull(meeting);
        Assert.Equal(9, meeting.StartTime.Hour); // Должна найти слот в 9:00
    }

    [Fact]
    public void ScheduleMeeting_OutsideBusinessHours_ShouldReturnNull()
    {
        // Arrange
        _service.CreateUser("Alice");
        
        var request = new MeetingRequest
        {
            ParticipantIds = new List<int> { 1 },
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 6, 20, 18, 0, 0, DateTimeKind.Utc), // После 17:00
            LatestEnd = new DateTime(2025, 6, 20, 19, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var meeting = _service.ScheduleMeeting(request);

        // Assert
        Assert.Null(meeting);
    }

    [Fact]
    public void ScheduleMeeting_WithNonExistentUser_ShouldReturnNull()
    {
        // Arrange
        var request = new MeetingRequest
        {
            ParticipantIds = new List<int> { 999 }, // Несуществующий пользователь
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var meeting = _service.ScheduleMeeting(request);

        // Assert
        Assert.Null(meeting);
    }

    [Fact]
    public void GetUserMeetings_ShouldReturnUserMeetings()
    {
        // Arrange
        _service.CreateUser("Alice");
        _service.CreateUser("Bob");
        
        var meeting = new Meeting
        {
            Id = 1,
            ParticipantIds = new List<int> { 1, 2 },
            StartTime = new DateTime(2025, 6, 20, 10, 0, 0, DateTimeKind.Utc),
            EndTime = new DateTime(2025, 6, 20, 11, 0, 0, DateTimeKind.Utc)
        };
        _service.AddMeeting(meeting);

        // Act
        var userMeetings = _service.GetUserMeetings(1);

        // Assert
        Assert.Single(userMeetings);
        Assert.Equal(1, userMeetings[0].Id);
    }

    [Fact]
    public void ScheduleMeeting_WithBackToBackMeetings_ShouldWorkCorrectly()
    {
        // Arrange
        _service.CreateUser("Alice");
        
        var request1 = new MeetingRequest
        {
            ParticipantIds = new List<int> { 1 },
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        };

        var request2 = new MeetingRequest
        {
            ParticipantIds = new List<int> { 1 },
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var meeting1 = _service.ScheduleMeeting(request1);
        var meeting2 = _service.ScheduleMeeting(request2);

        // Assert
        Assert.NotNull(meeting1);
        Assert.NotNull(meeting2);
        Assert.Equal(9, meeting1.StartTime.Hour);
        Assert.Equal(10, meeting2.StartTime.Hour); // Должна найти следующий слот
    }

    [Fact]
    public void ScheduleMeeting_WithNoAvailableSlot_ShouldReturnNull()
    {
        // Arrange
        _service.CreateUser("Alice");
        
        // Заполняем весь день встречами
        for (int i = 9; i < 17; i++)
        {
            var meeting = new Meeting
            {
                Id = i - 8,
                ParticipantIds = new List<int> { 1 },
                StartTime = new DateTime(2025, 6, 20, i, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2025, 6, 20, i + 1, 0, 0, DateTimeKind.Utc)
            };
            _service.AddMeeting(meeting);
        }

        var request = new MeetingRequest
        {
            ParticipantIds = new List<int> { 1 },
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var meeting = _service.ScheduleMeeting(request);

        // Assert
        Assert.Null(meeting);
    }
}

