using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MeetingScheduler.Models;
using MeetingScheduler.Services;
using System.Text.Json;
using System.Text;

namespace MeetingScheduler.Tests;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly Mock<ILogger<ScheduleService>> _loggerMock;

    public IntegrationTests()
    {
        _loggerMock = new Mock<ILogger<ScheduleService>>();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Удаляем существующий сервис, если он зарегистрирован
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ScheduleService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Регистрируем мок-сервис
                services.AddScoped<ScheduleService>(_ => new ScheduleService(_loggerMock.Object));
            });
        });
        
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
    
    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetUsers_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/users");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedUserId()
    {
        // Arrange
        var userName = "TestUser";
        var request = new { Name = userName };
        var content = new StringContent(
            JsonSerializer.Serialize(request), 
            Encoding.UTF8, 
            "application/json");

        // Act
        var response = await _client.PostAsync("/users", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, int>>(responseContent);
        
        Assert.NotNull(result);
        Assert.True(result.ContainsKey("id"));
        Assert.True(result["id"] > 0);
    }
    
    [Fact]
    public async Task GetUserById_ReturnsUser_WhenUserExists()
    {
        // Arrange - сначала создаем пользователя
        var userName = "TestUserForGet";
        var createRequest = new { Name = userName };
        var createContent = new StringContent(
            JsonSerializer.Serialize(createRequest), 
            Encoding.UTF8, 
            "application/json");
            
        var createResponse = await _client.PostAsync("/users", createContent);
        createResponse.EnsureSuccessStatusCode();
        
        var responseContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<Dictionary<string, int>>(responseContent);
        var userId = createResult["id"];

        // Act - получаем пользователя по ID
        var response = await _client.GetAsync($"/users/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var userContent = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<Dictionary<string, object>>(userContent);
        
        Assert.NotNull(user);
        Assert.True(user.ContainsKey("id"));
        Assert.True(user.ContainsKey("name"));
        Assert.Equal(userName, user["name"].ToString());
    }
    
    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenUserExists()
    {
        // Arrange - сначала создаем пользователя
        var userName = "TestUserForDelete";
        var createRequest = new { Name = userName };
        var createContent = new StringContent(
            JsonSerializer.Serialize(createRequest), 
            Encoding.UTF8, 
            "application/json");
            
        var createResponse = await _client.PostAsync("/users", createContent);
        createResponse.EnsureSuccessStatusCode();
        
        var responseContent = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonSerializer.Deserialize<Dictionary<string, int>>(responseContent);
        var userId = createResult["id"];

        // Act - удаляем пользователя
        var deleteResponse = await _client.DeleteAsync($"/users/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        
        // Проверяем, что пользователь действительно удален
        var getResponse = await _client.GetAsync($"/users/{userId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    #region UsersController Boundary Tests

    [Fact]
    public async Task CreateUser_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var request = new { Name = string.Empty };
        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WithVeryLongName_ReturnsBadRequest()
    {
        // Arrange
        var longName = new string('A', 501); // Имя длиннее 500 символов
        var request = new { Name = longName };
        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetNonExistentUser_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/users/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNonExistentUser_ReturnsNoContent()
    {
        // Act
        var response = await _client.DeleteAsync("/users/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion

    #region MeetingsController Boundary Tests

    [Fact]
    public async Task CreateMeeting_WithInvalidTimeRange_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            ParticipantIds = new[] { 1 },
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 12, 31, 17, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 12, 31, 9, 0, 0, DateTimeKind.Utc) // Ранее, чем EarliestStart
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/meetings", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMeeting_WithNonExistentParticipant_ReturnsNotFound()
    {
        // Arrange
        var request = new
        {
            ParticipantIds = new[] { 999999 },
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 12, 31, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 12, 31, 17, 0, 0, DateTimeKind.Utc)
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/meetings", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetNonExistentMeeting_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/meetings/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNonExistentMeeting_ReturnsNoContent()
    {
        // Act
        var response = await _client.DeleteAsync("/meetings/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetMeetingsForNonExistentUser_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/meetings/users/999999");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var meetings = JsonSerializer.Deserialize<List<object>>(content);
        
        Assert.NotNull(meetings);
        Assert.Empty(meetings);
    }

    [Fact]
    public async Task CreateMeeting_WithNoParticipants_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            ParticipantIds = Array.Empty<int>(),
            DurationMinutes = 60,
            EarliestStart = new DateTime(2025, 12, 31, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd = new DateTime(2025, 12, 31, 17, 0, 0, DateTimeKind.Utc)
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/meetings", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion
}
