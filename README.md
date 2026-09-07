# Vulnerable API Lab

Intentionally vulnerable full-stack user management API built for penetration testing, exploitation, remediation, and security testing practice.

The project simulates a small real-world API and is progressively developed to contain common web and API security vulnerabilities.

## Overview

The application consists of:

* ASP.NET Core Web API
* C#
* Entity Framework Core
* SQLite
* JavaScript frontend
* JWT authentication
* Role-based authorization
* REST API endpoints
* Intentionally vulnerable components for security testing

The goal is to follow a realistic penetration testing workflow rather than simply identifying vulnerabilities.

```text
Build → Identify → Hypothesize → Test → Exploit → Assess Impact → Remediate → Retest
```

## Architecture

```text
Browser
   │
   ▼
JavaScript Frontend
   │
   │ HTTP Requests
   ▼
ASP.NET Core Web API
   │
   ├── Authentication / Authorization
   │
   ├── Controllers
   │
   └── Entity Framework Core
           │
           ▼
        SQLite
```

## Security Testing

Security testing is performed manually using:

* Browser / Browser DevTools
* curl
* Burp Suite
* Manual HTTP requests
* JWT inspection and manipulation
* Request/response analysis

Testing focuses on understanding how the application behaves from an attacker's perspective.

## Confirmed Security Findings

| Finding                | Status                | Impact                                                                            |
| ---------------------- | --------------------- | --------------------------------------------------------------------------------- |
| BOLA / IDOR            | Confirmed             | Unauthorized access, modification, and deletion of other users                    |
| Mass Assignment        | Confirmed             | Unauthorized modification of user-controlled properties                           |
| Privilege Escalation   | Confirmed             | User account elevated to administrator through mass assignment                    |
| Information Disclosure | Confirmed             | Sensitive user data, including plaintext passwords, exposed through API responses |
| SQL Injection          | Remediated + Retested | Vulnerable query identified and successfully exploited before remediation         |
| JWT Security Testing   | Tested                | Token structure, claims, signature validation, and role claims analyzed           |
| Hardcoded JWT Secret   | Identified            | Sensitive signing key stored directly in application source code                  |

Additional vulnerabilities will be introduced and tested as the security lab develops.

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
| GET    | `/api/users/search`  | Search users by username       |

## Authentication & Authorization

The application uses JWT-based authentication.

Testing performed includes:

* Authentication flow testing
* JWT acquisition and inspection
* Bearer token handling
* Role-based authorization
* `401 Unauthorized` vs `403 Forbidden` behavior
* JWT claim inspection
* JWT payload tampering
* Signature validation
* Authorization boundary testing

Example authorization flow:

```text
Unauthenticated Request
        │
        ▼
      401
        │
        ▼
Authenticated User
        │
        ▼
   Role Validation
      /       \
    user     admin
     │          │
    403        200
```

## BOLA / IDOR Testing

The API was tested for Broken Object Level Authorization by manipulating object identifiers.

Testing demonstrated that an authenticated low-privileged user could:

* Read another user's data
* Modify another user's data
* Delete another user's account

This demonstrates an authorization failure at the object level.

## Mass Assignment & Privilege Escalation

The user creation endpoint was tested by supplying properties that should not have been controlled by the client.

By manipulating the `role` property, a newly created account could be assigned the `admin` role.

This was then used to demonstrate privilege escalation and access to administrator-only functionality.

```text
Normal User
    │
    │ role manipulation
    ▼
Admin User
    │
    ▼
Administrator-only functionality
```

## SQL Injection

A vulnerable user search endpoint was introduced for testing:

```http
GET /api/users/search?username=
```

The original implementation constructed SQL using direct string interpolation.

Testing demonstrated successful SQL injection using a URL-encoded payload.

The vulnerability was then remediated using a parameterized query with `SqliteParameter`.

The same injection payload was used during retesting.

```text
Vulnerable Query
      │
      ▼
SQL Injection Exploitation
      │
      ▼
Parameterized Query
      │
      ▼
Retest with Same Payload
      │
      ▼
Injection No Longer Successful
```

## Frontend

A lightweight JavaScript frontend is included to simulate interaction with the API through a browser.

The frontend is used for:

* User registration
* User login
* JWT-based authenticated requests
* API interaction
* Browser DevTools testing
* Burp Suite interception

## Tech Stack

### Backend

* ASP.NET Core
* C#
* Entity Framework Core
* SQLite

### Frontend

* HTML
* CSS
* JavaScript

### Security Testing

* Burp Suite
* Browser DevTools
* curl
* Manual HTTP requests

### Development

* Visual Studio Code
* Git
* GitHub

## Running Locally

### Start the API

```bash
dotnet restore
dotnet ef database update
dotnet run
```

The API runs locally on:

```text
http://localhost:5066
```

### Start the Frontend

From the project root:

```bash
cd frontend
python3 -m http.server 5500
```

The frontend is then available at:

```text
http://localhost:5500
```

## Project Structure

```text
vulnerable-api/
│
├── Controllers/
│   ├── AuthController.cs
│   └── UsersController.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── Models/
│   ├── User.cs
│   ├── LoginRequest.cs
│   ├── RegisterRequest.cs
│   └── CreateUserRequest.cs
│
├── Migrations/
│
├── frontend/
│   ├── index.html
│   ├── style.css
│   └── app.js
│
├── Program.cs
├── README.md
└── vulnerable-api.csproj
```

## Project Status

The application is being developed progressively as a practical penetration testing laboratory.

Current focus:

```text
Authentication
      ↓
Authorization
      ↓
API Security Testing
      ↓
Vulnerability Identification
      ↓
Exploitation
      ↓
Remediation
      ↓
Retesting
```

Upcoming security testing areas include:

* Cross-Site Scripting (XSS)
* Path Traversal
* Command Injection
* Insecure File Upload
* Rate Limiting weaknesses
* Additional JWT weaknesses
* Information disclosure scenarios
* Full security assessment

## Disclaimer

This application is intentionally vulnerable and is intended only for educational purposes and authorized security testing in a controlled local environment.

Do not deploy this application to a production or publicly accessible environment.
