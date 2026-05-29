# Authentication & Access Control

## Authentication

Bifrost uses **Google OAuth 2.0** for authentication. There are no local passwords — every identity is delegated to Google.

### Flow

```
Client                          Bifrost                        Google
  │                                │                               │
  │  1. Redirect user to Google    │                               │
  │ ─────────────────────────────────────────────────────────────► │
  │                                │                               │
  │  2. User authenticates, Google returns an authorization code   │
  │ ◄───────────────────────────────────────────────────────────── │
  │                                │                               │
  │  3. POST /auth/token { code }  │                               │
  │ ──────────────────────────────►│                               │
  │                                │  4. Exchange code for tokens  │
  │                                │ ─────────────────────────────►│
  │                                │  5. access_token, id_token …  │
  │                                │ ◄─────────────────────────────│
  │  6. OAuthToken (access_token)  │                               │
  │ ◄──────────────────────────────│                               │
  │                                │                               │
  │  7. Subsequent requests: Authorization: Bearer <access_token>  │
  │ ──────────────────────────────►│                               │
  │                                │  8. GET /userinfo             │
  │                                │ ─────────────────────────────►│
  │                                │  9. { id, email, name }       │
  │                                │ ◄─────────────────────────────│
  │                                │  10. Look up user by Google ID│
  │                                │      → load claims from DB    │
  │  11. Response                  │                               │
  │ ◄──────────────────────────────│                               │
```

### Step-by-step

| Step | Description |
|---|---|
| 1–2 | The client redirects the user to Google's consent screen. This part happens entirely on the client side. The authorization request **must include the `email` scope** — without it Google will not return the user's email in the `/userinfo` response and the server will reject the token with `401 Insufficient OAuth scope`. |
| 3 | Client sends the one-time authorization `code` to `POST /auth/token`. |
| 4–5 | `OAuthGateway.RequestToken` exchanges the code for tokens at `https://oauth2.googleapis.com/token` using the configured `client_id`, `client_secret`, and `redirect_uri`. |
| 6 | Bifrost returns the full `OAuthToken` to the client. The client stores the `access_token`. |
| 7 | All subsequent requests include `Authorization: Bearer <access_token>`. |
| 8–9 | `GoogleOAuthAuthenticationHandler` calls `https://www.googleapis.com/oauth2/v2/userinfo` on every authenticated request to resolve the Google identity. |
| 10 | The handler looks up the user in the database by `googleOpenid`. If no record is found the request is rejected with `401`. |
| 11 | On success, the handler builds a `ClaimsPrincipal` with the user's claims and the request proceeds. |

### Required Google OAuth scopes

The authorization request initiated by the client **must include at minimum**:

| Scope | Why it is required |
|---|---|
| `email` | `OAuthGateway.GetUserInfo` reads `email` from `/userinfo`. If absent, `InsufficientOAuthScopeException` is thrown and the token exchange fails with `401`. |
| `openid` | Required to obtain the Google user `id` used to look up the registered user in the database. |

Example authorization URL parameter: `scope=openid email`

---

### Required Google OAuth configuration (`appsettings.json`)

```json
"Google": {
  "OAuth": {
    "ClientId": "...",
    "ClientSecret": "...",
    "RedirectUri": "..."
  }
}
```

### Claims populated on authentication

| Claim | Source | Example value |
|---|---|---|
| `ClaimTypes.NameIdentifier` | `user.Id` (DB) | `3fa85f64-...` |
| `ClaimTypes.Email` | `user.Email` (DB) | `user@example.com` |
| `ClaimTypes.Role` | `user.Profile` | `Professor` / `Student` |
| `CourseId` | `user.CourseId` (DB) | `3fa85f64-...` or `""` |
| `IsAdmin` | `user.IsAdmin` (DB) | `True` / `False` |

> **Note:** authentication validates the token with Google on every request. There is no local session or JWT signing — the Google `access_token` is used directly as the Bearer credential.

---

## Authorization Policies

| Policy | Requirement | Who satisfies it |
|---|---|---|
| *(none)* | Valid Bearer token + user registered in DB | Any authenticated user |
| `AdminOnly` | Claim `IsAdmin = True` | Users with `IsAdmin = true` in the database |
| `ProfessorOnly` | Role claim = `Professor` | Users with `Profile = Professor` |
| `StudentOnly` | Role claim = `StudentOnly` | *(currently unused on any route)* |

---

## Route Access Matrix

`🔓 Public` — no token required  
`🔒 Authenticated` — valid Bearer token required (any registered user)  
`🛡 AdminOnly` — valid Bearer token **+** `IsAdmin = True`

### Authentication

| Method | Path | Access | Description |
|---|---|---|---|
| POST | `/auth/token` | 🔓 Public | Exchange Google authorization code for OAuth tokens |

### Courses

| Method | Path | Access | Description |
|---|---|---|---|
| GET | `/courses` | 🔒 Authenticated | Paginated list of courses |
| GET | `/courses/all` | 🔒 Authenticated | All courses (no pagination) |
| GET | `/courses/{id}` | 🔒 Authenticated | Get course by ID |
| POST | `/courses` | 🛡 AdminOnly | Create course |
| PUT | `/courses/{id}` | 🛡 AdminOnly | Update course |
| DELETE | `/courses/{id}` | 🛡 AdminOnly | Delete course |

### Assessment Seasons

| Method | Path | Access | Description |
|---|---|---|---|
| GET | `/assessment-seasons` | 🔒 Authenticated | Paginated list (filter by `courseId`) |
| GET | `/assessment-seasons/all` | 🔒 Authenticated | All seasons (no pagination) |
| GET | `/assessment-seasons/{id}` | 🔒 Authenticated | Get season by ID |
| POST | `/assessment-seasons` | 🛡 AdminOnly | Create season |
| PUT | `/assessment-seasons/{id}` | 🛡 AdminOnly | Update season |
| DELETE | `/assessment-seasons/{id}` | 🛡 AdminOnly | Delete season |

### Academic Centers

| Method | Path | Access | Description |
|---|---|---|---|
| GET | `/academic-centers` | 🔒 Authenticated | Paginated list (filter by `assessmentSeasonId`) |
| GET | `/academic-centers/{id}` | 🔒 Authenticated | Get academic center by ID |
| POST | `/academic-centers` | 🛡 AdminOnly | Create academic center |
| PUT | `/academic-centers/{id}` | 🛡 AdminOnly | Update academic center |
| DELETE | `/academic-centers/{id}` | 🛡 AdminOnly | Delete academic center |

### Coordinations

| Method | Path | Access | Description |
|---|---|---|---|
| GET | `/coordinations` | 🔒 Authenticated | Paginated list (filter by `assessmentSeasonId`) |
| GET | `/coordinations/{id}` | 🔒 Authenticated | Get coordination by ID |
| POST | `/coordinations` | 🛡 AdminOnly | Create coordination |
| PUT | `/coordinations/{id}` | 🛡 AdminOnly | Update coordination |
| DELETE | `/coordinations/{id}` | 🛡 AdminOnly | Delete coordination |

### Disciplines

| Method | Path | Access | Description |
|---|---|---|---|
| GET | `/disciplines` | 🔒 Authenticated | Paginated list |
| GET | `/disciplines/{id}` | 🔒 Authenticated | Get discipline by ID |
| POST | `/disciplines` | 🛡 AdminOnly | Create discipline |
| PUT | `/disciplines/{id}` | 🛡 AdminOnly | Update discipline |
| DELETE | `/disciplines/{id}` | 🛡 AdminOnly | Delete discipline |

### Users

| Method | Path | Access | Description |
|---|---|---|---|
| GET | `/users` | 🔒 Authenticated | Paginated list (filter by `profile`, `courseId`) |
| GET | `/users/me` | 🔒 Authenticated | Authenticated user's own profile |
| GET | `/users/{id}` | 🔒 Authenticated | Get user by ID |
| POST | `/users` | 🛡 AdminOnly | Create user |
| PUT | `/users/{id}` | 🛡 AdminOnly | Update user |
| DELETE | `/users/{id}` | 🛡 AdminOnly | Delete user |

---

## Error Responses

| Scenario | HTTP Status |
|---|---|
| No `Authorization` header | `401 Unauthorized` |
| Token rejected by Google (`/userinfo` fails) | `401 Unauthorized` |
| User not registered in the database | `401 Unauthorized` — `"User not registered."` |
| Authenticated but policy not satisfied (`AdminOnly`) | `403 Forbidden` |
