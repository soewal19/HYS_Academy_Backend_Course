# Meeting Scheduler

## Задача

Build a backend system to schedule meetings for multiple users without conflicts. You'll need to implement an algorithm that finds the earliest available time slot for a group of users given their existing schedules.

### Requirements

#### 1. Data Model:
- User: Id, Name
- Meeting: Id, Participants, StartTime, EndTime

#### 2. API Endpoints:
- POST /users
  - Body: { "name": "Alice" }
  - Creates a user
- POST /meetings
  - Body:
    {
      "participantIds": [1, 2, 3],
      "durationMinutes": 60,
      "earliestStart": "2025-06-20T09:00:00Z",
      "latestEnd": "2025-06-20T17:00:00Z"
    }
  - Returns the earliest time slot that fits all users' calendars
- GET /users/{userId}/meetings
  - Returns all meetings for a user

#### 3. Algorithm Challenge:
- Finds the earliest non-overlapping time slot that fits the duration and falls within the given day/time range
- Returns the proposed meeting time
- Edge cases: partial overlaps, back-to-back meetings, no available time slot

#### 4. Constraints:
- Business hours: 9:00–17:00 (UTC)
- In-memory data (no database required)
- ASP.NET Core

#### 5. Code Quality Expectations:
- Clean architecture encouraged
- Separation of concerns
- Unit-tested algorithm logic (mandatory)

### Тестирование

#### Модульные тесты
- **Тесты ScheduleService**: Полное покрытие алгоритма планирования, включая:
  - Граничные условия (минимальная/максимальная длительность встречи)
  - Проверки временных слотов
  - Обнаружение конфликтов
  - Планирование с несколькими участниками
  - Крайние случаи (нет доступных слотов, встречи подряд)

#### Интеграционные тесты
- **Тесты API эндпоинтов**:
  - Управление пользователями (создание, получение, удаление)
  - Планирование и получение встреч
  - Обработка невалидных данных
  - Крайние случаи (несуществующие пользователи/встречи, пустые списки участников)

#### Запуск тестов
```bash
# Запуск всех тестов
dotnet test

# Запуск конкретного тестового проекта
dotnet test Tests/Tests.csproj

# Запуск с подробным выводом
dotnet test --logger:"console;verbosity=detailed"
```

### Ограничения по времени
- Основная функциональность: ~2–3 часа
- Тестирование и покрытие кейсов: дополнительно 1-2 часа

### Отправка решения
- Репозиторий GitHub или архив
- README с инструкциями по настройке и известными ограничениями

### Результат работы программы

![backend](https://github.com/soewal19/HYS_Academy_Backend_Course/tree/main/img/1.png)


