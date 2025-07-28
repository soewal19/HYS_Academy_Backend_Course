# Changelog

## [Unreleased]

### Added
- Добавлены интеграционные тесты для API (UsersController, MeetingsController).
- Добавлены boundary-тесты для проверки граничных случаев в ScheduleService.
- Реализованы тесты для невалидных входных данных и несуществующих сущностей.

### Added
- Реализована поддержка одновременной работы с моковыми и реальными сервисами (DataProviderService, DATA_PROVIDER_MODE).
- Добавлен переключатель mock/backend режима через window.USE_MOCK_DATA (см. README).
- Обновлены README.md и README.en.md: подробные инструкции по режимам работы, моковым данным, возможностям приложения, интеграции с внешними календарями.
- Подготовлены отдельные инструкции по интеграции с Google Calendar и Outlook Calendar (OAuth2, REST API, client_id, scopes, примеры кода).
- MeetingsListComponent теперь работает с универсальным сервисом данных, не зависит от типа источника.
- Восстановлены отсутствующие файлы проекта: `MeetingScheduler.csproj`, `Tests.csproj`, `BackendCourse_2025Summer.sln`.
- Добавлен минимальный `Program.cs` для запуска ASP.NET Core Web API.
- Добавлен файл `CHANGELOG.md` для отслеживания изменений.
- Интегрирован Angular Signals во frontend (см. подробности ниже).

### Changed
- Проект мигрирован на .NET 8 (LTS): обновлены все TargetFramework и зависимости.
- Обновлена документация (`README.md`, `README.en.md`) под .NET 8, актуализированы команды запуска, тестирования, переменные окружения и ограничения.
- Исправлены ошибки в файле `launchSettings.json`.

### Fixed
- Исправлены ошибки сборки, связанные с несовместимостью NuGet-пакетов и отсутствием точки входа.
- Исправлено дублирование метода DeleteMeeting в ScheduleService.
- Исправлена инициализация ILogger в тестах.
- Исправлены тесты для учета начального состояния ScheduleService.

### Testing
- Добавлены модульные тесты для ScheduleService с покрытием граничных случаев.
- Реализованы интеграционные тесты для API (UsersController, MeetingsController).
- Добавлены тесты для обработки ошибок и невалидных данных.

### Frontend
- Добавлена демонстрационная интеграция Angular Signals для реактивности компонентов (см. `frontend/src/app/signals-demo/`).
- Обновлен пример использования Signals в документации frontend.

---

## Подробности по Signals
- Создан новый компонент `SignalsDemoComponent` для демонстрации реактивных сигналов.
- Используется `signal` из `@angular/core` для управления состоянием и реактивного UI.
- Пример: счетчик с использованием Signals, реактивное отображение значений.

---

## [Earlier]
- Начальная версия проекта: backend на ASP.NET Core, frontend на Angular, базовые API и UI.
