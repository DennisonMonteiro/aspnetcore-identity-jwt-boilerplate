<div align="center">

# 🚀 ASP.NET Core Identity JWT Boilerplate  

[![.NET](https://img.shields.io/badge/.NET-8.0-blue?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/Language-C%23-178600?logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
![Docker Ready](https://img.shields.io/badge/Docker-Ready-0db7ed?logo=docker&logoColor=white)
![Status](https://img.shields.io/badge/Status-Stable-success)
![Contributions](https://img.shields.io/badge/Contributions-Welcome-orange)

A modern **JWT Authentication and Identity boilerplate** built with **ASP.NET Core 8**, designed for quickly creating APIs with secure user login, token management, and admin seeding.

</div>

---

## ✨ Features

- ✅ ASP.NET Core 8.0 – lightweight and cross-platform
- ✅ Authentication via JWT (Bearer tokens)
- ✅ ASP.NET Core Identity integration
- ✅ Automatic database migrations on startup
- ✅ Admin user seeding (configured via `.env`)
- ✅ Docker Compose setup with SQL Server
- ✅ Swagger UI with JWT Auth support

---

## 🗂 Project Structure

```
src/
├── Identity.Api/          # API entrypoint
│   ├── Controllers/       # Auth and User endpoints
│   ├── Program.cs         # Main configuration
│   ├── appsettings.json   # Default config file
├── Identity.Data/         # EF Core + seed logic
├── Identity.Domain/       # Entity and domain models
```

---

## ⚙️ Environment Setup

### 1️⃣ Create a `.env` file in the root folder:

```bash
JWT_KEY=YourSuperSecretAndSuperSafeKey123!@#$%32CharacterMinimum
ADMIN_EMAIL=admin@identity.com
ADMIN_PASSWORD=Admin123!
```

### 2️⃣ Build & Run with Docker Compose

```bash
docker compose up -d --build
```

Then open [http://localhost:8080/swagger](http://localhost:8080/swagger)

✅ On first startup:
- Applies migrations automatically
- Creates the **admin user** (if it doesn’t exist)

You’ll see confirmation logs in the container output.

---

## 🧩 Default Credentials

| Key | Value |
|-----|-------|
| Admin Email | `admin@identity.com` |
| Admin Password | `Admin123!` |

---

## 🔐 API Endpoints

| Method | Endpoint              | Description                    | Auth Required |
|--------|------------------------|----------------------------------|---------------|
| `POST` | `/api/auth/register`  | Create a new user               | ❌ No         |
| `POST` | `/api/auth/login`     | Login and receive JWT token     | ❌ No         |
| `GET`  | `/api/user/me`        | Get logged user info            | ✅ Yes        |

```json
{
  "tokenType": "Bearer",
  "accessToken": "eyJhbGciOiJIUzI1...",
  "expiresIn": 3600
}
```

Use the token in future requests:

```
Authorization: Bearer YOUR_TOKEN_HERE
```

---

## 🧱 Database (Docker)
| Setting | Value |
|----------|-------|
| Server | `sql-server,1433` |
| Database | `identity-db` |
| User | `sa` |
| Password | `1q2w3e4r!@#` |

Persistent volume: `sql-server-mssql`

Connect manually:
```bash
sqlcmd -S localhost -U sa -P "1q2w3e4r!@#"
```

---

## 🧪 Local Development (Without Docker)

1. Start an instance of SQL Server locally.  
2. Update `appsettings.json` with your local connection string.  
3. Run the API manually:

```bash
dotnet run --project src/Identity.Api
```

Access locally via: [https://localhost:5001/swagger](https://localhost:5001/swagger)

---

## 🧰 Technologies Used

| Technology | Description |
|-------------|-------------|
| [.NET 8](https://dotnet.microsoft.com/) | Runtime and framework |
| [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity) | Authentication provider |
| [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) | ORM |
| [SQL Server 2022](https://hub.docker.com/_/microsoft-mssql-server) | Database |
| [Docker Compose](https://docs.docker.com/compose/) | Container orchestration |
| [JWT.io](https://jwt.io/) | Token verification tool |

---

## 📄 License

MIT License © 2025  
Feel free to fork, modify, and adapt this boilerplate for your own projects.

---

<div align="center">
💛 Created with passion using <b>ASP.NET Core + Identity + Docker</b> 💛  
</div>
