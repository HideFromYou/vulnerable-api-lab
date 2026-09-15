**# Vulnerable API Lab**

Intentionally vulnerable full-stack user management API built for penetration testing, exploitation, remediation, and security testing practice.

The project simulates a small real-world API and is progressively developed to contain common web and API security vulnerabilities.

The goal is to follow a realistic penetration testing workflow rather than simply identifying vulnerabilities.

```text
Build → Identify → Hypothesize → Test → Exploit → Assess Impact → Remediate → Retest
```

**---**

**## Overview**

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

The project is designed as a practical penetration testing laboratory where vulnerabilities are intentionally introduced, manually tested, exploited, remediated, and retested.

**---**

**## Architecture**

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

**---**

**## Security Testing**

Security testing is performed manually using:

- Browser / Browser DevTools
- curl
- Burp Suite
- Manual HTTP requests
- JWT inspection and manipulation
- Request / response analysis

Testing focuses on understanding how the application behaves from an attacker's perspective.

The project follows a practical methodology:

```text
Understand Application
        ↓
Identify Attack Surface
        ↓
Hypothesize
        ↓
Test
        ↓
Confirm Vulnerability
        ↓
Exploit
        ↓
Assess Impact
        ↓
Remediate
        ↓
Retest
```

**---**

**## Confirmed Security Findings**

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
| CSRF | Remediated + Retested | Authenticated state-changing request could initially be forged through a malicious page |
| SSRF | Remediated + Retested | Server-side URL fetching allowed access to an internal application endpoint |

Additional vulnerabilities will be introduced and tested as the security lab develops.

**---**

**## API Endpoints**

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
| POST | `/api/csrf/login` | Cookie-based CSRF demonstration login |
| POST | `/api/csrf/change-email` | CSRF testing endpoint for state-changing requests |
| GET | `/api/users/fetch` | Server-side URL fetching endpoint used for SSRF testing |

**---**

**## Authentication & Authorization**

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

**### Authorization Flow**

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
     user      admin
      │          │
     403        200
```

**---**

**## BOLA / IDOR Testing**

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

**### Testing Flow**

```text
Authenticated User
        │
        ▼
Access Own Object
        │
        ▼
Change Object ID
        │
        ▼
Access Another User's Object
        │
        ▼
Unauthorized Access
```

**---**

**## Mass Assignment & Privilege Escalation**

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

This demonstrated how mass assignment can lead to privilege escalation when sensitive authorization properties are accepted directly from the client.

**---**

**## SQL Injection**

A vulnerable user search endpoint was introduced for testing:

```http
GET /api/users/search?username=
```

The original implementation constructed SQL using direct string interpolation.

Testing demonstrated successful SQL injection using manually crafted input.

The SQL injection testing process included:

```text
Baseline Request
      ↓
Injection Character
      ↓
Boolean Testing
      ↓
Column Count Enumeration
      ↓
UNION-Based Testing
      ↓
Data Extraction
      ↓
Schema Enumeration
      ↓
Impact Assessment
```

**### Error-Based SQL Injection**

An initial injection character produced a database error.

The application exposed detailed development information through the response, including:

- Database exception
- SQL error
- Application stack trace
- Controller information
- Internal source path
- Source code line information

This demonstrated both SQL injection behavior and error information disclosure.

**### Boolean-Based SQL Injection**

Boolean conditions were used to compare application behavior between true and false SQL conditions.

```text
TRUE condition
    ↓
Expected query behavior
    ↓
FALSE condition
    ↓
Different application behavior
```

This allowed SQL injection to be confirmed without relying only on database errors.

**### Column Count Enumeration**

`ORDER BY` testing was used to determine the number of columns returned by the vulnerable query.

The application response changed when an invalid column position was supplied.

This established the number of columns required for subsequent UNION-based testing.

**### UNION-Based SQL Injection**

After determining the column count, UNION-based queries were used to identify which result positions were reflected into the API response.

A controlled query such as:

```sql
UNION SELECT 1,2,3,4,5
```

was used to map database result positions to API response fields.

**### Data Extraction**

After identifying useful result positions, the test was extended to retrieve records from the application's `Users` table.

**### Schema Enumeration**

SQLite's `sqlite_master` system table was used to enumerate database metadata.

Example:

```sql
SELECT name FROM sqlite_master;
```

The database schema was then queried to identify the structure of the `Users` table.

The testing demonstrated the typical SQL injection workflow:

```text
Confirm SQLi
    ↓
Determine Query Behavior
    ↓
Determine Column Count
    ↓
Identify UNION Positions
    ↓
Identify Tables
    ↓
Identify Columns
    ↓
Extract Relevant Data
    ↓
Assess Impact
```

**### Remediation**

The vulnerable string-interpolated query was replaced with a parameterized query using `SqliteParameter`.

The same injection payload was then used during retesting.

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

**---**

**## Cross-Site Scripting (XSS)**

Multiple XSS contexts were tested to understand how attacker-controlled input reaches different browser execution contexts.

Testing included:

- Stored XSS
- Reflected XSS
- DOM-based XSS
- JavaScript-context XSS
- HTML-context XSS

The testing demonstrated that XSS should be analyzed according to how attacker-controlled data flows through the application rather than treating it as a single vulnerability type.

```text
Source
   ↓
User-Controlled Data
   ↓
Data Flow
   ↓
Sink
   ↓
Execution Context
   ↓
JavaScript Execution
```

The stored and DOM-based XSS vulnerabilities were remediated by replacing unsafe HTML rendering with safer DOM APIs such as `textContent`.

**---**

**## Path Traversal**

A file retrieval endpoint was tested for directory traversal by manipulating the user-controlled filename parameter.

**### Baseline Request**

```http
GET /api/users/file?name=test.txt
```

Testing demonstrated that relative path traversal could escape the intended directory.

Example:

```http
GET /api/users/file?name=../secret.txt
```

This allowed retrieval of a file located outside the intended upload directory.

**### Testing Flow**

```text
Expected File
     ↓
Modify Filename
     ↓
Path Traversal Sequence
     ↓
Escape Intended Directory
     ↓
Access External File
```

**---**

**## Command Injection**

The API contains a ping endpoint that passes user-controlled input to a shell command.

**### Baseline Request**

```http
GET /api/users/ping?host=127.0.0.1
```

Testing demonstrated command injection by appending shell commands to the expected host value.

Example:

```http
GET /api/users/ping?host=127.0.0.1;whoami
```

The vulnerability was validated using harmless read-only commands in the local lab environment.

**### Testing Flow**

```text
Expected Host Input
       ↓
Modify Parameter
       ↓
Shell Metacharacter
       ↓
Additional Command
       ↓
Command Execution
```

**---**

**## Insecure File Upload**

The file upload endpoint was tested for insufficient file type validation.

Testing demonstrated that files with arbitrary extensions, including `.php`, could be uploaded and stored on the server filesystem.

The uploaded file was then verified through the application's file retrieval functionality.

The test demonstrated that accepting files primarily based on their filename extension can introduce security risks.

**---**

**## Information Disclosure**

API responses were tested for excessive exposure of sensitive information.

Testing demonstrated that user objects returned sensitive fields including plaintext passwords.

Example response:

```json
{
  "id": 3,
  "username": "george.alaman",
  "email": "alaman@example.com",
  "password": "password123",
  "role": "admin"
}
```

The upload functionality also returned the server-side filesystem path in its response.

This demonstrates the risk of returning internal or sensitive application data directly to clients.

**---**

**## Missing Rate Limiting**

The authentication endpoint was tested with repeated invalid login attempts.

Multiple consecutive failed authentication attempts returned:

```http
HTTP/1.1 401 Unauthorized
```

without visible throttling, account lockout, or rate limiting.

This demonstrates a potential brute-force protection weakness.

**### Testing Flow**

```text
Failed Login
     ↓
Failed Login
     ↓
Failed Login
     ↓
Repeated Requests Accepted
     ↓
No Visible Throttling
```

**---**

**## JWT Security Testing**

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

**### JWT Structure**

```text
Header.Payload.Signature
```

JWTs were analyzed as signed tokens rather than encrypted data.

```text
Header
   +
Payload
   +
Signature
   ↓
JWT
```

**### JWT Role Claim Testing**

The JWT role claim was manipulated during testing to determine whether changing the Base64URL-encoded payload alone would result in elevated privileges.

The modified token was rejected because the signature no longer matched the modified payload.

This demonstrated the importance of JWT signature validation.

**### Hardcoded JWT Secret**

The project also contains a hardcoded JWT signing secret for laboratory purposes.

This is intentionally insecure and exists to provide a realistic example of why cryptographic secrets should not be stored directly in application source code.

The secret is used only for the intentionally vulnerable local laboratory environment and should never be used in production.

**---**

**## Cross-Site Request Forgery (CSRF)**

A separate intentionally vulnerable cookie-based authentication flow was introduced to demonstrate classical CSRF.

The main application uses JWT authentication through the `Authorization: Bearer` header. Traditional CSRF is generally not applicable to this flow because the browser does not automatically attach the JWT as an `Authorization` header.

A dedicated cookie-based demonstration endpoint was therefore created for controlled testing.

**### CSRF Concept**

CSRF occurs when an attacker tricks an authenticated user's browser into sending an unwanted state-changing request to a vulnerable application.

The attack relies on authentication credentials being automatically included by the browser, such as session cookies.

```text
Victim logs in
      ↓
Browser stores authentication cookie
      ↓
Victim visits malicious page
      ↓
Malicious page submits forged request
      ↓
Browser automatically includes authentication cookie
      ↓
Server sees authenticated request
      ↓
Unauthorized action is performed
```

**### Vulnerable Endpoint**

The intentionally vulnerable endpoint was:

```http
POST /api/csrf/change-email
```

The endpoint accepted the state-changing request when the authentication cookie was present but initially had no CSRF protection.

**### Exploitation**

A malicious HTML page was created to submit a cross-origin form:

```html
<form action="http://localhost:5066/api/csrf/change-email" method="POST">
    <input type="hidden" name="email" value="attacker@example.com">
    <button type="submit">Change Email</button>
</form>
```

The authenticated browser submitted the request and the application returned:

```json
{
  "message": "Email changed to attacker@example.com"
}
```

This demonstrated successful CSRF exploitation.

**### Impact**

The attacker did not need to know the victim's password or session cookie.

The browser automatically supplied the authentication cookie, allowing the malicious page to trigger an authenticated state-changing request.

The impact depends on what state-changing functionality is exposed. In a real application, CSRF could potentially be used to:

- Change account information
- Change an email address
- Change account settings
- Perform transactions
- Modify security settings
- Trigger other authenticated actions

**### Remediation**

ASP.NET Core Anti-Forgery protection was introduced using `IAntiforgery`.

The protected endpoint validates the request using:

- Authentication cookie
- Anti-forgery cookie
- Valid anti-forgery request token

The request token is returned by the legitimate login flow and must be supplied by the legitimate client when performing the protected state-changing request.

The security model becomes:

```text
Authentication Cookie
        +
Valid CSRF Token
        ↓
Request Accepted
```

A forged request containing only the authentication cookie is rejected.

**### Retest**

The original malicious request was replayed after Anti-Forgery validation was enabled.

Without the required anti-forgery cookie/token, the request was rejected with an `AntiforgeryValidationException`.

A legitimate request containing both the required anti-forgery cookie and valid request token was then submitted through Burp Repeater.

The legitimate request returned:

```http
HTTP/1.1 200 OK
```

with:

```json
{
  "message": "Email changed to legitimate@example.com"
}
```

This demonstrated successful remediation and retesting:

```text
CSRF Vulnerability
       ↓
Successful Exploitation
       ↓
Anti-Forgery Protection
       ↓
Retest Malicious Request
       ↓
Request Rejected
       ↓
Retest Legitimate Request
       ↓
Request Accepted
```

**### CSRF vs JWT Authentication**

The project also demonstrates the difference between cookie-based authentication and JWT authentication.

With a traditional cookie-based session:

```text
Browser
   ↓
Automatically sends Cookie
   ↓
Authenticated Request
```

With the application's JWT flow:

```text
Client
   ↓
Explicitly adds:
Authorization: Bearer <JWT>
   ↓
Authenticated Request
```

A malicious website cannot normally cause the victim's browser to automatically attach an unknown JWT as an `Authorization` header.

Therefore, traditional CSRF is generally not applicable to the application's JWT Bearer authentication flow.

**---**

**## Server-Side Request Forgery (SSRF)**

A server-side URL fetching endpoint was introduced to demonstrate Server-Side Request Forgery (SSRF).

SSRF occurs when an application makes a server-side request to a URL controlled by the client. If the destination is insufficiently restricted, an attacker may cause the server to access internal or otherwise unintended resources.

**### Vulnerable Endpoint**

The intentionally vulnerable endpoint was:

```http
GET /api/users/fetch?url=https://example.com
```

The endpoint accepted a user-controlled URL and used the server's `HttpClient` to make the request.

**### Baseline Request**

A request to an external URL returned the remote content:

```http
GET /api/users/fetch?url=https://example.com
```

This confirmed that the application was performing the HTTP request server-side.

**### SSRF Exploitation**

A controlled internal endpoint was created inside the same application:

```http
GET /api/internal/secret
```

The vulnerable fetch endpoint was then supplied with the internal URL:

```http
GET /api/users/fetch?url=http://localhost:5066/api/internal/secret
```

The server made the request to the internal endpoint and returned its response to the attacker.

The internal service returned:

```json
{
  "service": "Internal Admin Service",
  "secret": "INTERNAL-SECRET-12345"
}
```

This demonstrated that attacker-controlled input could cause the application to access an internal resource that was not directly exposed through the intended URL-fetching functionality.

**### Testing Flow**

```text
Attacker
   ↓
Vulnerable URL Parameter
   ↓
Application Server
   ↓
Internal Resource
   ↓
Sensitive Response
   ↓
Attacker
```

**### Impact**

Successful SSRF can potentially allow an attacker to:

- Access internal application endpoints
- Reach services that are not intended to be externally accessible
- Retrieve sensitive internal data
- Interact with trusted internal services from the vulnerable server's network position

In this laboratory, the impact was demonstrated by accessing the controlled internal `/api/internal/secret` endpoint.

**### Remediation**

The vulnerable endpoint was remediated by validating the supplied URL before making the server-side request.

The implementation now:

- Requires a valid absolute URL
- Allows only the `https` scheme
- Uses an explicit hostname allowlist
- Rejects URLs outside the approved destination

The allowlist was intentionally restricted to:

```text
https://example.com
```

This prevents the endpoint from accepting arbitrary internal destinations such as the previously exploited localhost URL.

**### Retest**

The original SSRF payload was replayed after remediation:

```http
GET /api/users/fetch?url=http://localhost:5066/api/internal/secret
```

The request was rejected with:

```text
URL not allowed
```

A request to the allowed destination remained functional.

This demonstrated successful remediation and retesting:

```text
SSRF Vulnerability
       ↓
Successful Exploitation
       ↓
URL Validation / Allowlist
       ↓
Retest Original Payload
       ↓
Request Rejected
       ↓
Allowed Destination Retested
       ↓
Request Accepted
```

**---**

**## Frontend**

A lightweight JavaScript frontend is included to simulate interaction with the API through a browser.

The frontend is used for:

- User registration
- User login
- JWT-based authenticated requests
- API interaction
- Browser DevTools testing
- Burp Suite interception
- XSS testing
- CSRF demonstration

**---**

**## Tech Stack**

**### Backend**

- ASP.NET Core
- C#
- Entity Framework Core
- SQLite

**### Frontend**

- HTML
- CSS
- JavaScript

**### Security Testing**

- Burp Suite
- Browser DevTools
- curl
- Manual HTTP requests

**### Development**

- Visual Studio Code
- Git
- GitHub

**---**

**## Running Locally**

**### Start the API**

From the project root:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

The API runs locally on:

```text
http://localhost:5066
```

**### Start the Frontend**

From the project root:

```bash
cd frontend
python3 -m http.server 5500
```

The frontend is then available at:

```text
http://localhost:5500
```

**---**

**## Project Structure**

```text
vulnerable-api/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── CsrfController.cs
│   └── InternalController.cs
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
│   ├── csrf.html
│   ├── style.css
│   └── app.js
│
├── Program.cs
├── README.md
└── vulnerable-api.csproj
```

**---**

**## Project Status**

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
- CSRF
- SSRF

The lab will continue to evolve with additional vulnerabilities, remediation scenarios, retesting, and security assessment documentation.

**---**

**## Disclaimer**

This application is intentionally vulnerable and is intended only for educational purposes and authorized security testing in a controlled local environment.

Do not deploy this application to a production or publicly accessible environment.