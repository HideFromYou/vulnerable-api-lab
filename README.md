# Vulnerable API Lab

Intentionally vulnerable full-stack API built for penetration testing, exploitation, remediation, and security testing practice.

## Current Status

The project currently includes:

* ASP.NET Core Web API
* C#
* Entity Framework Core
* SQLite
* User CRUD operations
* Database migrations
* JWT Authentication
* Role-based Authorization
* Authentication and Authorization testing
* BOLA / IDOR testing
* Mass Assignment testing
* Privilege Escalation testing

## API Endpoints

| Method | Endpoint             | Description                    |
| ------ | -------------------- | ------------------------------ |
| POST   | `/api/auth/register` | Register a new user            |
| POST   | `/api/auth/login`    | Authenticate and receive a JWT |
| GET    | `/api/users`         | Get all users                  |
| GET    | `/api/users/{id}`    | Get a user by ID               |
| POST   | `/api/users`         | Create a user                  |
| PUT    | `/api/users/{id}`    | Update a user                  |
| DELETE | `/api/users/{id}`    | Delete a user                  |

## Security Lab

The application is progressively being developed into an intentionally vulnerable application.

The testing methodology follows:

```text
Build → Identify → Exploit → Remediate → Retest
```

Security testing is performed using:

* Browser
* curl
* Burp Suite
* Manual HTTP requests

### Confirmed Vulnerabilities

* BOLA / IDOR

  * Unauthorized read access
  * Unauthorized modification
  * Unauthorized deletion
* Mass Assignment
* Privilege Escalation

Additional vulnerabilities will be introduced and tested as the project develops.

## Tech Stack

* ASP.NET Core / C#
* Entity Framework Core
* SQLite
* JavaScript
* Burp Suite
* Git / GitHub

## Disclaimer

This application is intentionally vulnerable and is intended only for educational purposes and authorized security testing in a controlled local environment.
