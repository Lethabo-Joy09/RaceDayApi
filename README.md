# RaceDay API - Programming 2B (PROG6212)

## System Description

The RaceDay system is a comprehensive race event management platform connecting organisers and participants. Organisers create events, manage categories, view enrolments, and capture results. Participants browse events, enrol in categories, and track their personal results.

## User Roles

### Organiser
- Create, edit, and delete events
- Manage event categories and pricing
- View all participant enrolments
- Capture and manage race results

### Participant
- Create an account and manage profile
- Browse events by type (Run, Walk, Cycle)
- Enrol in events by selecting a category
- View their own enrolments and results

## Technology Stack

- **.NET 8.0** - Web API framework
- **Entity Framework Core 8.0** - Code-First database ORM
- **SQL Server LocalDB** - Database
- **Session-based authentication** - Cookie sessions with SHA256 password hashing
- **Swagger / OpenAPI** - Interactive API documentation
- **xUnit** - Unit testing framework
- **GitHub Actions** - CI/CD pipeline

## Database Design

8 interconnected tables: Users, Organisers, Participants, Categories, Events, EventCategories, Enrolments, Results.

<img width="1366" height="768" alt="ERD png" src="https://github.com/user-attachments/assets/33762cd6-d908-4f75-b32c-f26df8ecbf39" />


## API Endpoints

All endpoints are documented and testable through **Swagger UI** at `/swagger/index.html`.

### Authentication (Session-based)
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/logout
- GET /api/auth/me

### User Profile
- GET /api/users/profile
- PUT /api/users/profile

### Events (full CRUD)
- GET /api/events
- GET /api/events/{id}
- POST /api/events (Organiser only)
- PUT /api/events/{id} (Organiser only)
- DELETE /api/events/{id} (Organiser only)

### Categories (full CRUD)
- GET /api/categories
- POST /api/categories (Organiser only)
- PUT /api/categories/{id} (Organiser only)
- DELETE /api/categories/{id} (Organiser only)

### Enrolments
- GET /api/enrolments (Organiser only)
- GET /api/enrolments/my (Participant only)
- POST /api/enrolments (Participant only)
- PUT /api/enrolments/{id} (Organiser only)
- DELETE /api/enrolments/{id}

### Results
- GET /api/results/my (Participant only)
- GET /api/results/event/{eventId}
- POST /api/results/enrolment/{enrolmentId} (Organiser only)
- PUT /api/results/{id} (Organiser only)

### Dashboard
- GET /api/dashboard/organiser (Organiser only)
- GET /api/dashboard/participant (Participant only)

## Authentication

- Passwords hashed with SHA256 + salt
- Session cookies store UserId + Role
- Custom `[RoleAuthorize]` attribute enforces role access
- Session timeout: 60 minutes

## Unit Testing

**19 xUnit tests passing** across 4 test files:

- **JarredAuthTests** (5) - Registration, login, invalid credentials, duplicate emails, invalid roles
- **JarredRoleAccessTests** (5) - Role enforcement on endpoints
- **JarredEnrolmentTests** (3) - Enrolment scenarios
- **JarredUnauthenticatedTests** (6) - Anonymous access blocked
- <img width="1366" height="768" alt="Build-success png" src="https://github.com/user-attachments/assets/369bd9e5-3c97-4534-b403-4d1156f7c4d5" />


Run tests with:
```bash
dotnet test
