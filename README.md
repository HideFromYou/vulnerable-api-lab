# Vulnerable API Lab

Intentionally vulnerable full-stack API built for penetration testing, exploitation, remediation, and security testing practice.

## Current Status

The project currently includes:

- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQLite
- User CRUD operations
- Database migrations

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get a user by ID |
| POST | `/api/users` | Create a user |
| PUT | `/api/users/{id}` | Update a user |
| DELETE | `/api/users/{id}` | Delete a user |

## Security Lab

The application will progressively be developed into an intentionally vulnerable application.

The testing methodology will follow:

```text
Build → Identify → Exploit → Remediate → Retest

Security testing will be performed primarily with manual HTTP requests and Burp Suite.

Tech Stack
ASP.NET Core / C#
Entity Framework Core
SQLite
JavaScript
Burp Suite
Git / GitHub
Disclaimer

This application is intentionally vulnerable and is intended only for educational purposes and authorized security testing in a controlled local environment.