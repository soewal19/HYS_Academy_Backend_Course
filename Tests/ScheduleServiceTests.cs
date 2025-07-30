using System;
using System.Collections.Generic;
using MeetingScheduler.Models;
using MeetingScheduler.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MeetingScheduler.Tests;

public class ScheduleServiceTests
{
    private ScheduleService _service;
    private readonly Mock<ILogger<ScheduleService>> _loggerMock;

    public ScheduleServiceTests()
    {
        _loggerMock = new Mock<ILogger<ScheduleService>>();
    }

    private void InitializeService()
    {
        _service = new ScheduleService(_loggerMock.Object);
    }

    private void ResetService()
    {
        _service = null;
    }

    [Fact]
    public void CreateUser_ShouldReturnUserId()
    {
        // Arrange
        InitializeService();
        
        // Act
        var userId = _service.CreateUser("Alice");

        // Assert
        Assert.Equal(4, userId); // 3 пользователя уже созданы в конструкторе
        
        ResetService();
    }

    [Fact]
    public void ScheduleMeeting_WithAvailableTimeSlot_ShouldCreateMeeting()
    {
        // Arrange
        InitializeService();
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
        Assert.Equal(4, meeting.Id); // 3 встречи уже созданы в конструкторе
        Assert.Equal(2, meeting.ParticipantIds.Count);
        Assert.Equal(request.EarliestStart, meeting.StartTime);
        Assert.Equal(request.EarliestStart.AddMinutes(60), meeting.EndTime);
        
        ResetService();
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
        InitializeService();
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
        // Учитываем, что в конструкторе уже создаются встречи
        var expectedMeeting = userMeetings.FirstOrDefault(m => m.Id == 1);
        Assert.NotNull(expectedMeeting);
        
        ResetService();
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
        Assert.True(meeting1.EndTime <= meeting2.StartTime || 
                   meeting2.EndTime <= meeting1.StartTime);
    }

    [Fact]
    public void ScheduleMeeting_WithMinimumDuration_ShouldSucceed()
    {
        // Arrange
        InitializeService();
        _service.CreateUser("Alice");
        
        var request = new MeetingRequest
        {
            ParticipantIds = new List<int> { 1 },
            DurationMinutes = 1, // Минимальная длительность 1 минута
            EarliestStart = new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var meeting = _service.ScheduleMeeting(request);

        // Assert
        Assert.NotNull(meeting);
        Assert.Equal(TimeSpan.FromMinutes(1), meeting.EndTime - meeting.StartTime);
        
        ResetService();
    }

    [Fact]
    public void ScheduleMeeting_WithMaximumDuration_ShouldSucceed()
    {
        // Arrange
        InitializeService();
        _service.CreateUser("Alice");
        
        var request = new MeetingRequest
        {
            ParticipantIds = new List<int> { 1 },
            DurationMinutes = 8 * 60, // Максимальная длительность - рабочий день (8 часов)
            EarliestStart = new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var meeting = _service.ScheduleMeeting(request);

        // Assert
        Assert.NotNull(meeting);
        Assert.Equal(TimeSpan.FromHours(8), meeting.EndTime - meeting.StartTime);
        
        ResetService();
    }

    [Fact]
    public void ScheduleMeeting_AtEarliestPossibleTime_ShouldSucceed()
    {
        // Arrange
        InitializeService();
        _service.CreateUser("Alice");
        
        var earliestStart = new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc);
        var request = new MeetingRequest
        {
            ParticipantIds = new List<int> { 1 },
            DurationMinutes = 60,
            EarliestStart = earliestStart,
            LatestEnd = new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var meeting = _service.ScheduleMeeting(request);

        // Assert
        Assert.NotNull(meeting);
        Assert.Equal(earliestStart, meeting.StartTime);
        
        ResetService();
    }

    [Fact]
    public void ScheduleMeeting_AtLatestPossibleTime_ShouldSucceed()
    {
        // Arrange
        InitializeService();
        _service.CreateUser("Alice");
        
        var latestEnd = new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc);
        var request = new MeetingRequest
        {
            ParticipantIds = new List<int> { 1 },
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = latestEnd
        };

        // Act
        var meeting = _service.ScheduleMeeting(request);

        // Assert
        Assert.NotNull(meeting);
        Assert.Equal(latestEnd, meeting.EndTime);
        
        ResetService();
    }

    [Fact]
    public void ScheduleMeeting_WithMultipleParticipants_ShouldFindCommonSlot()
    {
        // Arrange
        InitializeService();
        _service.CreateUser("Alice");
        _service.CreateUser("Bob");
        _service.CreateUser("Charlie");
        
        // Создаем встречи для первых двух пользователей
        _service.ScheduleMeeting(new MeetingRequest
        {
            ParticipantIds = new List<int> { 1, 2 },
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 6, 20, 10, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 6, 20, 11, 0, 0, DateTimeKind.Utc)
        });

        var request = new MeetingRequest
        {
            ParticipantIds = new List<int> { 1, 2, 3 }, // Третий пользователь свободен
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var meeting = _service.ScheduleMeeting(request);

        // Assert
        Assert.NotNull(meeting);
        // Проверяем, что встреча не пересекается с существующей
        Assert.True(meeting.EndTime <= new DateTime(2025, 6, 20, 10, 0, 0, DateTimeKind.Utc) ||
                   meeting.StartTime >= new DateTime(2025, 6, 20, 11, 0, 0, DateTimeKind.Utc));
        
        ResetService();
    }

    [Fact]
    public void ScheduleMeeting_WithNoAvailableSlots_ShouldReturnNull()
    {
        // Arrange
        InitializeService();
        _service.CreateUser("Alice");
        
        // Заполняем весь рабочий день встречами
        for (int hour = 9; hour < 17; hour++)
        {
            _service.ScheduleMeeting(new MeetingRequest
            {
                ParticipantIds = new List<int> { 1 },
                DurationMinutes = 60,
                EarliestStart = new DateTime(2025, 6, 20, hour, 0, 0, DateTimeKind.Utc),
                LatestEnd = new DateTime(2025, 6, 20, hour + 1, 0, 0, DateTimeKind.Utc)
            });
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
        
        ResetService();
    }

    [Fact]
    public void ScheduleMeeting_WithNoAvailableSlot_ShouldReturnNull()
    {
        // Arrange
        InitializeService();
        _service.CreateUser("Alice");
        
        // Заполняем весь день встречами
        for (int i = 9; i < 17; i++)
        {
            var meetingSlot = new Meeting
            {
                Id = i - 8,
                ParticipantIds = new List<int> { 1 },
                StartTime = new DateTime(2025, 6, 20, i, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2025, 6, 20, i + 1, 0, 0, DateTimeKind.Utc)
            };
            _service.AddMeeting(meetingSlot);
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

    [Fact]
    public void CreateAndDeleteMeeting_ShouldWorkCorrectly()
    {
        // Arrange
        InitializeService();
        _service.CreateUser("Alice");
        _service.CreateUser("Bob");
        
        // Создаем встречу напрямую, чтобы избежать влияния ScheduleMeeting
        var meeting = new Meeting
        {
            Id = 100, // Используем большой ID, чтобы не пересекаться с существующими
            ParticipantIds = new List<int> { 1, 2 },
            StartTime = new DateTime(2025, 6, 20, 10, 0, 0, DateTimeKind.Utc),
            EndTime = new DateTime(2025, 6, 20, 11, 0, 0, DateTimeKind.Utc)
        };
        _service.AddMeeting(meeting);
        
        // Act
        _service.DeleteMeeting(meeting.Id);
        
        // Assert
        var userMeetings = _service.GetUserMeetings(1);
        Assert.DoesNotContain(meeting, userMeetings);
        
        ResetService();
    }
}

