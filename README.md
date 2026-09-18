# Vulnerable API Lab

Intentionally vulnerable full-stack banking-style web application and REST API built for penetration testing, exploitation, remediation, retesting, and security testing practice.

The project simulates a small real-world application called **NovaBank** and is progressively developed to contain common web application and API security vulnerabilities.

The goal is to follow a realistic penetration testing workflow rather than simply identifying vulnerabilities.

~~~text
Build → Identify → Hypothesize → Test → Exploit → Assess Impact → Remediate → Retest
~~~

---

## Overview

The application consists of:

- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQLite
- JavaScript frontend
- JWT authentication
- Role-based authorization
- REST-style API endpoints
- User management functionality
- Banking-style account dashboard
- Transfers
- Transaction history
- Security Center
- Document upload functionality
- Intentionally vulnerable components for security testing

The project is designed as a practical penetration testing laboratory where vulnerabilities are intentionally introduced, manually tested, exploited, remediated, and retested.

The application combines both backend API security testing and browser-based application testing.

The frontend provides a realistic attack surface for testing:

- Authentication
- Authorization
- Session behavior
- API requests
- Object-level access control
- Input validation
- File upload functionality
- XSS
- CSRF
- Transaction functionality
- Browser DevTools
- Burp Suite interception

---

## Application Architecture

~~~text
Browser
   │
   ▼
NovaBank JavaScript Frontend
   │
   │ HTTP Requests
   ▼
ASP.NET Core Web API
   │
   ├── Authentication / Authorization
   │
   ├── User Management
   │
   ├── Transactions
   │
   ├── File Upload
   │
   ├── Security Testing Endpoints
   │
   └── Entity Framework Core
            │
            ▼
          SQLite
~~~

The application is intentionally designed so that security testing can be performed from the perspective of an authenticated and unauthenticated attacker.

---

## Request Flow

~~~text
Browser
   ↓
JavaScript
   ↓
Fetch API
   ↓
HTTP Request
   ↓
ASP.NET Core
   ↓
Controller
   ↓
Application Logic
   ↓
Entity Framework Core
   ↓
SQLite Database
   ↓
HTTP Response
   ↓
Browser
~~~

Example login flow:

~~~text
User clicks Login
       ↓
app.js catches event
       ↓
fetch()
       ↓
POST /api/auth/login
       ↓
AuthController
       ↓
AppDbContext
       ↓
SQLite
       ↓
Credentials validated
       ↓
JWT generated
       ↓
JSON response
       ↓
JavaScript stores token
       ↓
Authenticated API requests
~~~

---

## Security Testing Methodology

Security testing is performed manually using:

- Browser / Browser DevTools
- Burp Suite
- curl
- Manual HTTP requests
- JWT inspection and manipulation
- Request / response analysis
- Database behavior analysis
- Source-code review
- Controlled exploitation

The general methodology is:

~~~text
Understand Application
        ↓
Identify Attack Surface
        ↓
Identify Input
        ↓
Understand Data Flow
        ↓
Create Hypothesis
        ↓
Establish Baseline
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
~~~

The project focuses on understanding:

- Why a vulnerability exists
- Where attacker-controlled input enters
- How the input travels through the application
- Which component processes the input
- Where the security decision occurs
- What the final security sink is
- What impact the vulnerability creates
- How the vulnerability can be remediated
- How the fix can be verified through retesting

---

# Security Testing Coverage

## 20 / 20 Checked

| # | Finding | Status |
|---|---|---|
| 01 | BOLA / IDOR | ✅ |
| 02 | Mass Assignment | ✅ |
| 03 | Privilege Escalation | ✅ |
| 04 | Information Disclosure | ✅ |
| 05 | SQL Injection | ✅ |
| 06 | Stored XSS | ✅ |
| 07 | Reflected XSS | ✅ |
| 08 | DOM XSS | ✅ |
| 09 | JavaScript-Context XSS | ✅ |
| 10 | HTML-Context XSS | ✅ |
| 11 | Path Traversal | ✅ |
| 12 | Command Injection | ✅ |
| 13 | Insecure File Upload | ✅ |
| 14 | Upload Filename Path Traversal | ✅ |
| 15 | Server-Side Code Execution | ✅ |
| 16 | Missing Rate Limiting | ✅ |
| 17 | JWT Security Testing | ✅ |
| 18 | Hardcoded JWT Secret | ✅ |
| 19 | CSRF | ✅ |
| 20 | SSRF | ✅ |

---

# API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Authenticate and receive JWT |
| GET | `/api/users` | Get users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create user |
| PUT | `/api/users/{id}` | Update user |
| DELETE | `/api/users/{id}` | Delete user |
| GET | `/api/users/search` | Search users by username |
| GET | `/api/users/reflect` | Reflected XSS testing endpoint |
| GET | `/api/users/js` | JavaScript-context XSS testing endpoint |
| GET | `/api/users/file` | File retrieval endpoint |
| GET | `/api/users/ping` | Command injection testing endpoint |
| POST | `/api/users/upload` | File upload endpoint |
| GET | `/api/users/fetch` | Server-side URL fetching / SSRF endpoint |
| POST | `/api/csrf/login` | Cookie-based CSRF demonstration login |
| POST | `/api/csrf/change-email` | CSRF testing endpoint |
| GET | `/api/transactions` | Retrieve transaction history |
| POST | `/api/transactions` | Create transaction / transfer |
| POST | `/api/execution/run` | Controlled server-side C# execution endpoint |

---

# Authentication & Authorization

The application uses JWT-based authentication.

Testing included:

- Authentication flow
- JWT acquisition
- JWT inspection
- Bearer token handling
- Role-based authorization
- `401 Unauthorized`
- `403 Forbidden`
- JWT claim inspection
- JWT payload manipulation
- Signature validation
- Authorization boundary testing
- Privilege escalation

Authentication and authorization are treated as separate concepts:

~~~text
Authentication
"Who are you?"
        ↓
JWT
        ↓
Authorization
"What are you allowed to do?"
~~~

---

# 01 — BOLA / IDOR

Broken Object Level Authorization was tested by manipulating object identifiers supplied by the client.

An authenticated low-privileged user was able to interact with another user's object by changing the object ID.

Example:

~~~http
GET /api/users/3
~~~

Testing flow:

~~~text
Authenticated User
        ↓
Access Own Object
        ↓
Change Object ID
        ↓
Access Another Object
        ↓
Unauthorized Access
~~~

The test demonstrated unauthorized:

- Data access
- Data modification
- Object deletion

Root cause:

**Authentication was implemented, but object-level authorization was insufficient.**

---

# 02 — Mass Assignment

Mass Assignment was tested by supplying properties that should not have been controlled by the client.

The `role` property was accepted through client-controlled input.

Example:

~~~json
{
  "username": "nikos",
  "email": "nikos@test.local",
  "role": "admin"
}
~~~

Testing flow:

~~~text
Client-Controlled Property
        ↓
Model Binding
        ↓
Sensitive Server-Side Property
        ↓
Unauthorized State Change
~~~

The vulnerability demonstrated why sensitive authorization properties should not be directly bindable from untrusted client input.

---

# 03 — Privilege Escalation

Mass Assignment was used to manipulate the user's authorization state.

A normal user role was changed to:

~~~text
admin
~~~

A fresh login then generated a JWT containing the administrative role.

Testing flow:

~~~text
Normal User
     ↓
Manipulate role
     ↓
Role becomes admin
     ↓
Fresh Login
     ↓
JWT with admin role
     ↓
Admin functionality
~~~

This demonstrated a privilege escalation chain originating from unsafe client-controlled authorization data.

---

# 04 — Information Disclosure

API responses were tested for excessive exposure of sensitive information.

Sensitive user fields were exposed in API responses, including plaintext passwords.

Example:

~~~json
{
  "id": 3,
  "username": "george.alaman",
  "email": "alaman@example.com",
  "password": "password123",
  "role": "admin"
}
~~~

The upload functionality also returned the server-side filesystem path.

Detailed development errors also exposed internal application information during SQL injection testing.

The testing demonstrated why API responses should return only the information required by the client.

---

# 05 — SQL Injection

A vulnerable endpoint was introduced:

~~~http
GET /api/users/search?username=
~~~

The vulnerable implementation constructed SQL using user-controlled input.

The testing methodology included:

~~~text
Baseline
   ↓
Injection Character
   ↓
Boolean Testing
   ↓
ORDER BY Enumeration
   ↓
Column Count
   ↓
UNION Testing
   ↓
Database Enumeration
   ↓
Schema Enumeration
   ↓
Data Extraction
   ↓
Impact Assessment
~~~

SQLite metadata was also enumerated through:

~~~sql
SELECT name FROM sqlite_master;
~~~

The vulnerable query was later remediated using a parameterized query with `SqliteParameter`.

The same injection attempts were then retested against the remediated implementation.

---

# 06 — Stored XSS

Stored Cross-Site Scripting was tested by storing attacker-controlled input in application data.

The malicious value was stored in the database and later returned through an API response.

The frontend initially rendered the returned value using an unsafe HTML sink.

Testing flow:

~~~text
Attacker Input
      ↓
Database
      ↓
API Response
      ↓
Frontend
      ↓
innerHTML
      ↓
Browser parses HTML
      ↓
JavaScript execution
~~~

The vulnerability demonstrated the importance of output encoding and safe DOM APIs.

The vulnerable rendering was later remediated and retested.

---

# 07 — Reflected XSS

Reflected XSS was tested through:

~~~http
GET /api/users/reflect?input=
~~~

Baseline:

~~~http
GET /api/users/reflect?input=HELLO
~~~

The parameter was reflected directly in the HTTP response.

A controlled HTML payload was then supplied.

The browser interpreted the reflected markup and executed the JavaScript payload.

The testing demonstrated:

~~~text
Request
   ↓
Server
   ↓
HTTP Response
   ↓
Browser
   ↓
Execution
~~~

---

# 08 — DOM XSS

A dedicated DOM XSS flow was introduced into the frontend.

The vulnerable JavaScript obtained attacker-controlled data from the URL fragment:

~~~javascript
const domInput = decodeURIComponent(window.location.hash.substring(1));
~~~

The data was then inserted into the DOM using:

~~~javascript
profileResult.innerHTML += domInput;
~~~

The source-to-sink flow was:

~~~text
Attacker-Controlled URL Fragment
            ↓
window.location.hash
            ↓
decodeURIComponent()
            ↓
domInput
            ↓
innerHTML
            ↓
Browser DOM
            ↓
HTML parsing
            ↓
JavaScript execution
~~~

This was later remediated using safer DOM handling and retested.

---

# 09 — JavaScript-Context XSS

The JavaScript-context endpoint was:

~~~http
GET /api/users/js?name=
~~~

Baseline:

~~~http
GET /api/users/js?name=XSS_TEST_123
~~~

The response contained:

~~~javascript
<script>let username = "XSS_TEST_123";</script>
~~~

The input therefore appeared inside a double-quoted JavaScript string.

A controlled test payload was used to break out of the string context and inject JavaScript.

The vulnerable response became conceptually:

~~~javascript
<script>let username = "";alert(1);//";</script>
~~~

The testing demonstrated the key principle:

**XSS payload construction depends on the output context.**

---

# 10 — HTML-Context XSS

The reflected endpoint was tested for direct HTML interpretation:

~~~http
GET /api/users/reflect?input=HTML_TEST_123
~~~

The response contained the user-controlled value inside an HTML document.

A controlled HTML payload using an event handler was then tested.

The browser parsed the HTML and triggered the event handler.

Testing demonstrated:

~~~text
Input
 ↓
HTML Response
 ↓
HTML Parser
 ↓
DOM
 ↓
Event Handler
 ↓
JavaScript Execution
~~~

This exercise was used to distinguish HTML-context XSS from JavaScript-context XSS.

---

# XSS Methodology

The project demonstrates that XSS should be analyzed in two dimensions.

## Delivery Type

~~~text
Reflected
Request → Response → Browser

Stored
Request → Database → Response → Browser

DOM
Attacker Input → JavaScript → DOM Sink → Browser
~~~

## Context

Examples include:

~~~text
HTML Context
Attribute Context
JavaScript Context
URL Context
CSS Context
DOM Context
~~~

The key pentesting questions are:

- Where does the attacker-controlled input originate?
- How does it travel?
- What parser processes it?
- What is the final sink?
- What context is the value interpreted in?

---

# 11 — Path Traversal

A file retrieval endpoint was tested:

~~~http
GET /api/users/file?name=test.txt
~~~

Baseline:

~~~http
GET /api/users/file?name=test.txt
~~~

The application was then tested with relative path traversal.

Example:

~~~http
GET /api/users/file?name=../secret.txt
~~~

The request returned a file outside the intended upload directory.

Testing flow:

~~~text
Expected Filename
      ↓
../
      ↓
Parent Directory
      ↓
Escape Intended Directory
      ↓
Read External File
~~~

A controlled local file was also created to practice target discovery and traversal calculation.

The exercise demonstrated the importance of:

- Locating the intended directory
- Locating the target file
- Calculating traversal depth
- Confirming arbitrary file access

---

# 12 — Command Injection

The vulnerable endpoint was:

~~~http
GET /api/users/ping?host=
~~~

Baseline input:

~~~text
127.0.0.1
~~~

The endpoint was then tested using shell metacharacters.

Example:

~~~http
GET /api/users/ping?host=;whoami
~~~

The server executed the additional command.

Additional shell operators were tested to understand parser behavior:

~~~text
;   command separator
&&  execute next command if previous succeeds
||  execute next command if previous fails
|   pipe output to another command
&   background operator
~~~

Example concepts:

~~~text
Input
  ↓
Shell Parser
  ↓
Shell Operator
  ↓
Additional Command
  ↓
Command Execution
~~~

The exercises were performed using harmless commands in the local laboratory.

---

# 13 — Insecure File Upload

The upload endpoint was discovered:

~~~http
POST /api/users/upload
~~~

An initial request without a file exposed the expected multipart field:

~~~text
file
~~~

Baseline upload:

~~~http
POST /api/users/upload
Content-Type: multipart/form-data
~~~

A normal text file was uploaded successfully.

The filename was then changed to a different extension such as:

~~~text
path.php
~~~

The server accepted and stored the file.

The test demonstrated that arbitrary file extensions could be accepted without sufficiently strong server-side validation.

Important lesson:

**Successful file upload does not require server-side execution to be a security issue.**

---

# 14 — Upload Filename Path Traversal

The upload functionality was also tested using a manipulated filename containing parent-directory traversal.

Example concept:

~~~text
../filename
~~~

The vulnerable implementation accepted the traversal sequence in the filename/path handling flow.

This scenario was included in the laboratory to demonstrate why upload filenames should never be trusted as filesystem paths.

Security testing focused on:

- Filename normalization
- Directory boundaries
- Path joining
- Canonicalization
- Destination directory enforcement

The test remains part of the project's security coverage.

---

# 15 — Server-Side Code Execution

A dedicated intentionally vulnerable execution endpoint was created:

~~~http
POST /api/execution/run?path=<file-path>
~~~

The vulnerable controller performs:

~~~text
User-Controlled Path
        ↓
File.ReadAllTextAsync(path)
        ↓
Read C# Source
        ↓
CSharpScript.EvaluateAsync()
        ↓
Server-Side Code Execution
~~~

A controlled local test file was created containing:

~~~csharp
return "UPLOAD_RCE_TEST";
~~~

The endpoint returned:

~~~text
UPLOAD_RCE_TEST
~~~

This demonstrated controlled server-side C# execution within the local laboratory.

The scenario illustrates the potential escalation chain:

~~~text
Insecure File Upload
        ↓
File Stored on Server
        ↓
Executable Content
        ↓
Execution Endpoint
        ↓
Server-Side Code Execution
~~~

This component exists exclusively for controlled security testing and must not be exposed to an untrusted network.

---

# 16 — Missing Rate Limiting

The authentication endpoint was tested with repeated invalid login attempts.

Multiple consecutive failures returned:

~~~http
HTTP/1.1 401 Unauthorized
~~~

without visible:

- Request throttling
- Account lockout
- Rate limiting
- Increasing delay

Testing flow:

~~~text
Failed Login
     ↓
Failed Login
     ↓
Failed Login
     ↓
Repeated Requests Accepted
     ↓
No Visible Throttling
~~~

The laboratory therefore demonstrates a potential brute-force protection weakness.

---

# 17 — JWT Security Testing

JWTs were analyzed from both an authentication and authorization perspective.

Testing included:

- Header inspection
- Payload inspection
- Claims
- Role claims
- Expiration
- Bearer token handling
- Payload manipulation
- Signature validation
- Authorization behavior

JWT structure:

~~~text
Header.Payload.Signature
~~~

JWTs were treated as signed tokens rather than encrypted data.

The role claim was modified during testing.

Changing the Base64URL-encoded payload without recalculating the correct signature caused the modified token to be rejected.

This demonstrated the role of the JWT signature in token integrity.

---

# 18 — Hardcoded JWT Secret

The project contains a deliberately hardcoded JWT signing secret for laboratory purposes.

The goal is to demonstrate why cryptographic secrets should not be embedded directly in application source code.

Security lesson:

~~~text
Application Source
      ↓
Hardcoded Secret
      ↓
Secret Exposure Risk
      ↓
Potential Token Forgery
~~~

In a production environment, secrets should be managed through appropriate secret-management mechanisms and should not be committed to source control.

---

# 19 — CSRF

A separate cookie-based authentication scenario was created to demonstrate Cross-Site Request Forgery.

The endpoint was:

~~~http
POST /api/csrf/change-email
~~~

The vulnerable flow was:

~~~text
Victim logs in
      ↓
Browser stores authentication cookie
      ↓
Victim visits malicious page
      ↓
Malicious page submits forged request
      ↓
Browser automatically sends cookie
      ↓
Server sees authenticated request
      ↓
State-changing action occurs
~~~

A controlled malicious HTML form was used during testing.

The request was then protected with ASP.NET Core Anti-Forgery functionality using `IAntiforgery`.

The remediated model requires:

~~~text
Authentication Cookie
        +
Valid CSRF Token
        ↓
Request Accepted
~~~

The malicious request without the required anti-forgery token was rejected.

A legitimate request containing the appropriate anti-forgery values was accepted.

The vulnerability was therefore remediated and retested.

### JWT vs Cookie Authentication

The main NovaBank API uses:

~~~http
Authorization: Bearer <JWT>
~~~

The browser does not normally attach this header automatically to arbitrary cross-origin requests.

The dedicated CSRF demonstration therefore uses a cookie-based authentication flow to illustrate traditional CSRF behavior.

---

# 20 — SSRF

A server-side URL fetching endpoint was introduced:

~~~http
GET /api/users/fetch?url=https://example.com
~~~

The application uses the server's HTTP client to retrieve the requested URL.

Baseline:

~~~http
GET /api/users/fetch?url=https://example.com
~~~

A controlled internal endpoint was also created:

~~~http
GET /api/internal/secret
~~~

The vulnerable fetch functionality was then supplied with a localhost destination.

Example:

~~~http
GET /api/users/fetch?url=http://localhost:5066/api/internal/secret
~~~

The server made the request to the internal endpoint and returned the response.

Controlled internal response:

~~~json
{
  "service": "Internal Admin Service",
  "secret": "INTERNAL-SECRET-12345"
}
~~~

Testing flow:

~~~text
Attacker
   ↓
User-Controlled URL
   ↓
Application Server
   ↓
Internal Resource
   ↓
Sensitive Response
   ↓
Attacker
~~~

The vulnerable implementation was then remediated through URL validation and an explicit destination allowlist.

The internal localhost request was subsequently rejected.

Legitimate requests to the approved destination remained functional.

---

# Frontend

The project includes a browser-based NovaBank frontend designed to simulate a small banking application.

The frontend includes:

- Registration
- Login
- Dashboard
- My Account
- Transfers
- Transaction history
- Security Center
- Document upload
- Logout
- JWT-based authentication

The frontend communicates with the API using JavaScript and the Fetch API.

---

## app.js

The `app.js` file contains important client-side application logic.

It is responsible for:

- Event listeners
- Login requests
- Registration requests
- JWT handling
- API calls
- DOM manipulation
- User interface updates
- Transaction requests
- Upload requests

For penetration testing, client-side JavaScript is useful because it can reveal:

~~~text
Endpoints
   ↓
HTTP Methods
   ↓
Parameters
   ↓
Request Headers
   ↓
Authentication
   ↓
Client-Side Logic
   ↓
DOM Sources
   ↓
DOM Sinks
~~~

Client-side code should not be treated as the only source of truth.

Endpoint discovery can combine:

- JavaScript inspection
- Network tab
- Burp Suite
- Content discovery
- Backend source-code review

---

# DOM XSS Sources & Sinks

Examples of common client-side sources:

- `window.location.hash`
- `window.location.search`
- `window.location.href`
- `document.URL`
- `document.referrer`
- `postMessage`
- `localStorage`

Examples of dangerous DOM sinks:

- `innerHTML`
- `outerHTML`
- `insertAdjacentHTML`
- `document.write`

Safer text rendering example:

~~~javascript
element.textContent = input;
~~~

Unsafe HTML rendering example:

~~~javascript
element.innerHTML = input;
~~~

The important pentester question is:

~~~text
Where does the attacker-controlled data come from?
        ↓
How is it processed?
        ↓
Where does it end?
        ↓
What parser interprets it?
~~~

---

# Transactions Backend

The project contains a banking-style transaction system.

Functionality includes:

- Transaction model
- Transaction request model
- EF Core mapping
- Transaction controller
- Database migration
- Transaction creation
- Transaction history

Main endpoints:

~~~http
GET /api/transactions
POST /api/transactions
~~~

Example transaction request:

~~~json
{
  "recipientId": 2,
  "amount": 100,
  "description": "Test transfer"
}
~~~

Potential future testing areas include:

- BOLA / IDOR
- Authorization bypass
- Business logic flaws
- Parameter tampering
- Transaction integrity
- Negative amounts
- Unexpected values
- Recipient manipulation
- Race conditions
- Sensitive transaction data exposure

---

# NovaBank Security Center

The Security Center provides security-related account functionality.

It includes:

- Password status
- Authentication status
- Session status
- Uploaded documents

The file upload flow is:

~~~text
NovaBank Dashboard
       ↓
Security Center
       ↓
Uploaded Documents
       ↓
Upload Document
       ↓
File Input
       ↓
POST /api/users/upload
       ↓
Server Filesystem
~~~

This creates a realistic browser-based entry point for testing upload security.

---

# Project Structure

~~~text
vulnerable-api/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── TransactionsController.cs
│   ├── CsrfController.cs
│   ├── InternalController.cs
│   └── ExecutionController.cs
│
├── Data/
│   └── AppDbContext.cs
│
├── Models/
│   ├── User.cs
│   ├── LoginRequest.cs
│   ├── RegisterRequest.cs
│   ├── CreateUserRequest.cs
│   ├── Transaction.cs
│   └── CreateTransactionRequest.cs
│
├── Migrations/
│   ├── InitialCreate
│   ├── addUserRole
│   ├── AddUserPassword
│   ├── AddTransactions
│   └── AppDbContextModelSnapshot.cs
│
├── frontend/
│   ├── index.html
│   ├── csrf.html
│   ├── style.css
│   └── app.js
│
├── uploads/
│
├── Program.cs
├── README.md
├── appsettings.json
├── appsettings.Development.json
├── vulnerable-api.csproj
└── vulnerable-api.http
~~~

---

# Important Components

## Program.cs

Responsible for:

- Application startup
- Service registration
- Entity Framework Core
- SQLite
- JWT authentication
- Authorization
- CORS
- Middleware
- OpenAPI
- HTTP pipeline

---

## Controllers

Controllers receive HTTP requests and return HTTP responses.

Examples:

~~~text
AuthController
UsersController
TransactionsController
CsrfController
InternalController
ExecutionController
~~~

---

## AppDbContext

Entity Framework Core provides the bridge between C# application objects and the SQLite database.

Conceptually:

~~~text
C# Application
     ↓
Entity Framework Core
     ↓
SQL
     ↓
SQLite
~~~

Example:

~~~csharp
public DbSet<User> Users { get; set; }
~~~

---

# Technology Stack

## Backend

- ASP.NET Core
- .NET 10
- C#
- Entity Framework Core
- SQLite
- JWT Authentication
- ASP.NET Core Anti-Forgery
- C# scripting for controlled execution testing

## Frontend

- HTML5
- CSS3
- JavaScript
- Fetch API
- Browser DOM APIs

## Security Testing

- Burp Suite
- Browser DevTools
- curl
- Manual HTTP requests
- JWT inspection
- Request / response analysis
- Source-code review

## Development

- Visual Studio Code
- .NET SDK
- Git
- GitHub

---

# Running Locally

## Start Backend

From the project root:

~~~bash
dotnet restore
dotnet ef database update
dotnet run
~~~

API:

~~~text
http://localhost:5066
~~~

## Start Frontend

~~~bash
cd frontend
python3 -m http.server 5500
~~~

Frontend:

~~~text
http://localhost:5500
~~~

---

# Git / GitHub Workflow

The project is version-controlled with Git.

Typical workflow:

~~~bash
git status
git add .
git commit -m "Update vulnerable API lab"
git push origin main
~~~

Repository:

~~~text
https://github.com/HideFromYou/vulnerable-api-lab
~~~

The project is intended to evolve continuously as additional vulnerabilities, business-logic scenarios, remediation exercises, and retesting activities are added.

---

# Development Workflow

Each feature follows a security-oriented process:

~~~text
Application Feature
       ↓
Understand Functionality
       ↓
Identify Attack Surface
       ↓
Identify Inputs
       ↓
Trace Data Flow
       ↓
Create Hypothesis
       ↓
Establish Baseline
       ↓
Manual Testing
       ↓
Confirm Vulnerability
       ↓
Controlled Exploitation
       ↓
Impact Assessment
       ↓
Remediation
       ↓
Retesting
~~~

This approach allows the project to function as both:

- A full-stack development exercise
- A practical penetration testing laboratory

---

# Pentester Mindset

The project emphasizes asking the following questions during testing:

~~~text
Where is the input?
        ↓
Who controls it?
        ↓
Where does it go?
        ↓
Which component processes it?
        ↓
What parser interprets it?
        ↓
What is the final sink?
        ↓
Can I access something I should not?
        ↓
Can I modify something I should not?
        ↓
Can I execute something I should not?
~~~

The core security-testing mindset is:

**Input → Context → Parser → Sink → Security Impact**

---

# Project Status

Current application functionality includes:

- User registration
- JWT authentication
- Role-based authorization
- User management
- NovaBank dashboard
- Account section
- Transfer functionality
- Transaction history
- Security Center
- Document upload
- CSRF demonstration flow
- SSRF demonstration flow
- Controlled server-side execution testing

Current security coverage:

- ✅ BOLA / IDOR
- ✅ Mass Assignment
- ✅ Privilege Escalation
- ✅ Information Disclosure
- ✅ SQL Injection
- ✅ Stored XSS
- ✅ Reflected XSS
- ✅ DOM XSS
- ✅ JavaScript-context XSS
- ✅ HTML-context XSS
- ✅ Path Traversal
- ✅ Command Injection
- ✅ Insecure File Upload
- ✅ Upload Filename Path Traversal
- ✅ Server-Side Code Execution
- ✅ Missing Rate Limiting
- ✅ JWT Security Testing
- ✅ Hardcoded JWT Secret
- ✅ CSRF
- ✅ SSRF

---

# Future Work

The laboratory will continue to evolve with additional:

- Authentication vulnerabilities
- Authorization vulnerabilities
- API security issues
- Business logic vulnerabilities
- Transaction security testing
- Race conditions
- Security misconfigurations
- Additional XSS contexts
- File handling scenarios
- Request smuggling scenarios
- HTTP security testing
- Advanced JWT scenarios
- Remediation exercises
- Retesting exercises
- Security reporting
- Professional penetration testing documentation

The objective is to gradually transform the project into a more complete full-stack penetration testing laboratory.

---

# Educational Purpose

This project was created to develop practical knowledge in:

- Web application security
- REST API security
- ASP.NET Core
- C#
- Entity Framework Core
- SQLite
- Authentication
- Authorization
- JWT
- Burp Suite
- Browser DevTools
- Manual request manipulation
- Vulnerability identification
- Exploitation
- Remediation
- Retesting
- Security documentation

The project emphasizes understanding the root cause of vulnerabilities rather than relying only on automated scanners.

---

# Disclaimer

This application is intentionally vulnerable and is intended only for educational purposes and authorized security testing in a controlled local environment.

Do not deploy this application to a production or publicly accessible environment.

The vulnerabilities and server-side execution functionality contained in this project are intentionally implemented for penetration testing practice.

Any security testing against systems that you do not own or do not have explicit authorization to test is outside the intended scope of this project.

---

# Author

**Nikos Alaman**

Cybersecurity / Penetration Testing Learning Project

Technologies and security concepts practiced:

**ASP.NET Core · C# · EF Core · SQLite · REST APIs · JWT · JavaScript · Burp Suite · Web Application Security · API Security · Penetration Testing**