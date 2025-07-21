# Meeting Scheduler Application

## Описание
Полноценное приложение для планирования встреч между несколькими пользователями без конфликтов по времени. Включает backend на ASP.NET Core и frontend на Angular с Tailwind CSS.

## Структура проекта

```
BackendCourse_2025Summer/
├── Controllers/           # API endpoints (Meetings, Users)
├── Models/                # Модели данных (User, Meeting, MeetingRequest)
├── Services/              # Бизнес-логика (ScheduleService)
├── Tests/                 # Unit-тесты для алгоритма планирования
├── frontend/              # Angular-приложение (UI, сервисы, модели)
├── CI_CD/                 # Скрипты и конфиги для CI/CD
│   ├── github-actions-dotnet.yml # Пример workflow для GitHub Actions
│   └── azure-pipelines.yml      # Пример pipeline для Azure Pipelines
├── Dockerfile             # Docker-образ для backend
├── .env                   # Переменные окружения
└── README.md              # Документация
```

## Как работает приложение
- Пользователь может создать себя через POST /users
- Можно создать встречу для группы пользователей через POST /meetings — система найдет первый доступный слот без пересечений
- Можно получить список встреч пользователя через GET /users/{userId}/meetings
- Вся логика поиска слота реализована в сервисе ScheduleService (учет пересечений, рабочие часы, edge cases)
- Все данные хранятся в памяти (in-memory)

## Запуск приложения
### Backend
1. Перейдите в корень проекта
2. Запустите backend:
   ```bash
   dotnet run
   ```
   Backend будет доступен на http://localhost:5000

### Frontend
1. Перейдите в папку frontend:
   ```bash
   cd frontend
   npm install
   npm start
   ```
   Приложение будет доступно на http://localhost:4200

### Docker
1. Соберите и запустите контейнер:
   ```bash
   docker build -t meeting-scheduler .
   docker run -p 5000:5000 meeting-scheduler
   ```

### CI/CD
- Все скрипты и конфиги для CI/CD находятся в папке `CI_CD/`
- Примеры workflow:
  - GitHub Actions: `CI_CD/github-actions-dotnet.yml`
  - Azure Pipelines: `CI_CD/azure-pipelines.yml`
- Автоматическая сборка, тестирование и деплой при пуше в main

## Переменные окружения (.env)
```
ASPNETCORE_URLS=http://+:5000
# Дополнительные переменные при необходимости
```

## API Endpoints
- POST /users — создать пользователя
- POST /meetings — создать встречу (алгоритм поиска слота)
- GET /users/{userId}/meetings — получить встречи пользователя

## Ограничения
- Все данные теряются при перезапуске (in-memory)
- Нет аутентификации/авторизации

## Тестирование
- Unit-тесты для алгоритма планирования находятся в папке `Tests/`
- Запуск тестов:
  ```bash
  dotnet test
  ```

## Лицензия
MIT

---

**English version available in [README.en.md](README.en.md)** 