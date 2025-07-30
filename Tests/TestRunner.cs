using Xunit;
using Xunit.Abstractions;

namespace MeetingScheduler.Tests;

public class TestRunner
{
    private readonly ITestOutputHelper _output;

    public TestRunner(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void RunAllTests()
    {
        _output.WriteLine("Запуск всех тестов...");
        
        // Создаем экземпляр тестового класса
        var tests = new IntegrationTests();
        
        // Запускаем тесты последовательно
        tests.GetUsers_ReturnsSuccessStatusCode().GetAwaiter().GetResult();
        tests.CreateUser_ReturnsCreatedUserId().GetAwaiter().GetResult();
        tests.GetUserById_ReturnsUser_WhenUserExists().GetAwaiter().GetResult();
        tests.DeleteUser_ReturnsNoContent_WhenUserExists().GetAwaiter().GetResult();
        
        _output.WriteLine("Все тесты выполнены успешно!");
    }
}
