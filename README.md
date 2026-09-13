# Vulnerable API Lab

Intentionally vulnerable full-stack user management API built for penetration testing, exploitation, remediation, and security testing practice.

The project simulates a small real-world API and is progressively developed to contain common web and API security vulnerabilities.

## Overview

The application consists of:

- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQLite
- JavaScript frontend
- JWT authentication
- Role-based authorization
- REST API endpoints
- Intentionally vulnerable components for security testing

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

- Browser / Browser DevTools
- curl
- Burp Suite
- Manual HTTP requests
- JWT inspection and manipulation
- Request/response analysis

Testing focuses on understanding how the application behaves from an attacker's perspective.

## Confirmed Security Findings

| Finding | Status | Impact |
| --- | --- | --- |
| BOLA / IDOR | Confirmed | Unauthorized access, modification, and deletion of other users |
| Mass Assignment | Confirmed | Unauthorized modification of user-controlled properties |
| Privilege Escalation | Confirmed | User account elevated to administrator through mass assignment |
| Information Disclosure | Confirmed | Sensitive user data, including plaintext passwords, exposed through API responses |
| SQL Injection | Remediated + Retested | Vulnerable query identified and successfully exploited before remediation |
| Stored XSS | Remediated + Retested | Malicious user input executed through unsafe frontend HTML rendering |
| Reflected XSS | Confirmed | User-controlled input reflected into an HTML response and executed |
| DOM XSS | Remediated + Retested | User-controlled URL input reached a vulnerable DOM sink |
| JavaScript-context XSS | Confirmed | User input escaped a JavaScript string context and executed attacker-controlled code |
| Path Traversal | Confirmed | Relative path manipulation allowed access to a file outside the intended directory |
| Command Injection | Confirmed | User-controlled input was incorporated into a shell command |
| Insecure File Upload | Confirmed | Arbitrary file extensions were accepted and stored on the server |
| Missing Rate Limiting | Confirmed | Repeated failed authentication attempts were accepted without throttling or lockout |
| JWT Security Testing | Tested | Token structure, claims, signature validation, and role claims analyzed |
| Hardcoded JWT Secret | Identified | Sensitive signing key stored directly in application source code |

Additional vulnerabilities will be introduced and tested as the security lab develops.

## API Endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Authenticate and receive a JWT |
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get a user by ID |
| POST | `/api/users` | Create a user |
| PUT | `/api/users/{id}` | Update a user |
| DELETE | `/api/users/{id}` | Delete a user |
| GET | `/api/users/search` | Search users by username |
| GET | `/api/users/reflect` | XSS testing endpoint |
| GET | `/api/users/js` | JavaScript-context XSS testing endpoint |
| GET | `/api/users/file` | File retrieval endpoint |
| GET | `/api/users/ping` | Command injection testing endpoint |
| POST | `/api/users/upload` | File upload endpoint |

## Authentication & Authorization

The application uses JWT-based authentication.

Testing performed includes:

- Authentication flow testing
- JWT acquisition and inspection
- Bearer token handling
- Role-based authorization
- `401 Unauthorized` vs `403 Forbidden` behavior
- JWT claim inspection
- JWT payload tampering
- Signature validation
- Authorization boundary testing
- Privilege escalation testing

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

- Read another user's data
- Modify another user's data
- Delete another user's account

This demonstrates an authorization failure at the object level.

The vulnerability was identified by changing the object ID in requests such as:

```http
GET /api/users/3
```

The API authenticated the requester but failed to verify whether the requester was authorized to access the requested object.

## Mass Assignment & Privilege Escalation

The user creation and update endpoints were tested by supplying properties that should not have been controlled by the client.

By manipulating the `role` property, an authenticated user could modify their own role and escalate from a normal user to an administrator.

Example:

```json
{
  "username": "nikos",
  "email": "nikos@test.local",
  "role": "admin"
}
```

After changing the role in the database, a fresh login generated a new JWT containing the `admin` role.

The new token was then used to access administrator-only functionality.

```text
Normal User
    │
    │ role manipulation
    ▼
Admin User
    │
    │ fresh authentication
    ▼
JWT with admin role
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

## Cross-Site Scripting (XSS)

Multiple XSS contexts were tested to understand how attacker-controlled input reaches different browser execution contexts.

Testing included:

- Stored XSS
- Reflected XSS
- DOM-based XSS
- JavaScript-context XSS
- HTML-context XSS

The stored and DOM-based XSS vulnerabilities were remediated by replacing unsafe HTML rendering with safer DOM APIs such as `textContent`.

The testing demonstrated the importance of understanding:

```text
Source → Data Flow → Sink → Execution Context
```

rather than treating XSS as a single vulnerability type.

## Path Traversal

A file retrieval endpoint was tested for directory traversal by manipulating the user-controlled filename parameter.

Baseline request:

```http
GET /api/users/file?name=test.txt
```

Testing demonstrated that relative path traversal could escape the intended directory:

```http
GET /api/users/file?name=../secret.txt
```

This allowed retrieval of a file located outside the intended upload directory.

## Command Injection

The API contains a ping endpoint that passes user-controlled input to a shell command.

Baseline request:

```http
GET /api/users/ping?host=127.0.0.1
```

Testing demonstrated command injection by appending shell commands to the expected host value.

Example:

```http
GET /api/users/ping?host=127.0.0.1;whoami
```

The vulnerability was validated using harmless read-only commands in the local lab environment.

## Insecure File Upload

The file upload endpoint was tested for insufficient file type validation.

Testing demonstrated that files with arbitrary extensions, including `.php`, could be uploaded and stored on the server filesystem.

The uploaded file was then verified through the application's file retrieval functionality.

The test demonstrated that accepting files based primarily on their filename extension can introduce security risks.

## Information Disclosure

API responses were tested for excessive exposure of sensitive information.

Testing demonstrated that user objects returned sensitive fields including plaintext passwords.

The upload functionality also returned the server-side filesystem path in its response.

This demonstrates the risk of returning internal or sensitive application data directly to clients.

## Missing Rate Limiting

The authentication endpoint was tested with repeated invalid login attempts.

Multiple consecutive failed authentication attempts returned:

```http
HTTP/1.1 401 Unauthorized
```

without visible throttling, account lockout, or rate limiting.

This demonstrates a potential brute-force protection weakness.

## JWT Security Testing

JWT authentication was tested from both an authentication and authorization perspective.

Testing included:

- JWT structure inspection
- Header and payload analysis
- Claim inspection
- Role claim manipulation
- Signature validation
- Expiration claim inspection
- Bearer token handling
- Authorization behavior based on JWT claims

The project also contains a hardcoded JWT signing secret for laboratory purposes.

JWTs were treated as signed tokens rather than encrypted data.

```text
Header
   +
Payload
   +
Signature
   ↓
JWT
```

## Frontend

A lightweight JavaScript frontend is included to simulate interaction with the API through a browser.

The frontend is used for:

- User registration
- User login
- JWT-based authenticated requests
- API interaction
- Browser DevTools testing
- Burp Suite interception
- XSS testing

## Tech Stack

### Backend

- ASP.NET Core
- C#
- Entity Framework Core
- SQLite

### Frontend

- HTML
- CSS
- JavaScript

### Security Testing

- Burp Suite
- Browser DevTools
- curl
- Manual HTTP requests

### Development

- Visual Studio Code
- Git
- GitHub

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
Impact Assessment
      ↓
Remediation
      ↓
Retesting
```

Current security testing coverage includes:

- BOLA / IDOR
- Mass Assignment
- Privilege Escalation
- Information Disclosure
- SQL Injection
- Stored XSS
- Reflected XSS
- DOM XSS
- JavaScript-context XSS
- HTML-context XSS
- Path Traversal
- Command Injection
- Insecure File Upload
- Missing Rate Limiting
- JWT Security Testing
- Hardcoded JWT Secret

The lab will continue to evolve with additional vulnerabilities, remediation scenarios, retesting, and security assessment documentation.

## Disclaimer

This application is intentionally vulnerable and is intended only for educational purposes and authorized security testing in a controlled local environment.

Do not deploy this application to a production or publicly accessible environment.