# Swagger API Documentation / Документация Swagger

## How Swagger Works (English)
Swagger (OpenAPI) automatically generates interactive documentation for your REST API. When you add Swashbuckle.AspNetCore to your ASP.NET Core project and enable Swagger, it scans your controllers and models, and creates a web page (Swagger UI) where you can:
- See all available endpoints (routes, methods, parameters, responses)
- Try out API requests directly from the browser (with real data)
- See example requests and responses
- Share your API documentation with frontend developers or testers

Swagger UI is available at `/swagger` (e.g., http://localhost:5000/swagger) after you run your backend.

## Как работает Swagger (на русском)
Swagger (OpenAPI) автоматически генерирует интерактивную документацию для вашего REST API. После подключения Swashbuckle.AspNetCore к ASP.NET Core проекту и включения Swagger:
- Все контроллеры и модели сканируются, и на специальной странице (`/swagger`) появляется список всех доступных endpoint-ов (маршрутов, методов, параметров, ответов)
- Вы можете отправлять реальные запросы к API прямо из браузера (Try it out)
- Видны примеры запросов и ответов
- Документацию можно отправить фронтенд-разработчикам или тестировщикам

Swagger UI доступен по адресу `/swagger` (например, http://localhost:5000/swagger) после запуска backend.

---

## Как открыть Swagger UI / How to open Swagger UI

1. Запустите backend / Run backend:
   ```
   dotnet run --project MeetingScheduler
   ```
2. Откройте браузер и перейдите по адресу / Open browser and go to:
   ```
   http://localhost:5000/swagger
   ```

3. Вы увидите интерактивную документацию для всех endpoint-ов (POST /users, POST /meetings, GET /users/{userId}/meetings и т.д.)
   /
   You will see interactive documentation for all endpoints (POST /users, POST /meetings, GET /users/{userId}/meetings, etc.)

## Пример запроса создания пользователя / Example user creation request

POST /users
```json
{
  "name": "Alice"
}
```

## Пример запроса создания встречи / Example meeting creation request

POST /meetings
```json
{
  "participantIds": [1, 2, 3],
  "durationMinutes": 60,
  "earliestStart": "2025-06-20T09:00:00Z",
  "latestEnd": "2025-06-20T17:00:00Z"
}
```

---

## Как подключить Swagger к ASP.NET Core / How to add Swagger to ASP.NET Core

1. Добавьте пакет NuGet / Add NuGet package:
   ```
   dotnet add package Swashbuckle.AspNetCore
   ```
2. В Program.cs / In Program.cs:
   ```csharp
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen();
   ...
   app.UseSwagger();
   app.UseSwaggerUI();
   ```
3. Перезапустите backend и откройте /swagger.
   / Restart backend and open /swagger.
озда