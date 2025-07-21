# Meeting Scheduler Application

## Overview
A full-featured application for scheduling meetings between multiple users without time conflicts. Includes a backend on ASP.NET Core and a frontend on Angular with Tailwind CSS.

## Project Structure
```
BackendCourse_2025Summer/
├── Controllers/           # API endpoints (Meetings, Users)
├── Models/                # Data models (User, Meeting, MeetingRequest)
├── Services/              # Business logic (ScheduleService)
├── Tests/                 # Unit tests for scheduling algorithm
├── frontend/              # Angular app (UI, services, models)
├── CI_CD/                 # CI/CD scripts and configs
│   ├── github-actions-dotnet.yml # Example workflow for GitHub Actions
│   └── azure-pipelines.yml      # Example pipeline for Azure Pipelines
├── Dockerfile             # Docker image for backend
├── .env                   # Environment variables
└── README.md              # Documentation
```

## How the Application Works
- Users can be created via POST /users
- Meetings for a group of users can be scheduled via POST /meetings — the system finds the earliest available slot without conflicts
- All meetings for a user can be retrieved via GET /users/{userId}/meetings
- The slot-finding logic is implemented in the ScheduleService (handles conflicts, business hours, edge cases)
- All data is stored in-memory

## Running the Application
### Backend
1. Go to the project root
2. Start the backend:
   ```bash
   dotnet run
   ```
   Backend will be available at http://localhost:5000

### Frontend
1. Go to the frontend folder:
   ```bash
   cd frontend
   npm install
   npm start
   ```
   The app will be available at http://localhost:4200

### Docker
1. Build and run the container:
   ```bash
   docker build -t meeting-scheduler .
   docker run -p 5000:5000 meeting-scheduler
   ```

### CI/CD
- All scripts and configs for CI/CD are in the `CI_CD/` folder
- Example workflows:
  - GitHub Actions: `CI_CD/github-actions-dotnet.yml`
  - Azure Pipelines: `CI_CD/azure-pipelines.yml`
- Automatic build, test, and deploy on push to main

## Environment Variables (.env)
```
ASPNETCORE_URLS=http://+:5000
# Add other variables as needed
```

## API Endpoints
- POST /users — create a user
- POST /meetings — create a meeting (slot-finding algorithm)
- GET /users/{userId}/meetings — get meetings for a user

## Limitations
- All data is lost on restart (in-memory)
- No authentication/authorization

## Testing
- Unit tests for the scheduling algorithm are in the `Tests/` folder
- Run tests:
  ```bash
  dotnet test
  ```

## License
MIT 