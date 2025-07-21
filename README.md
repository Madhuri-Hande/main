# .NET Core Web API Project

## 📦 Project Overview
This project is a .NET Core Web API application that provides a set of RESTful endpoints for managing expenses and user authentication. It is built using modern development practices and tools to ensure scalability, security, and maintainability.

## 🚀 Features
- RESTful API endpoints for CRUD operations
- Entity Framework Core with Code First approach
- JWT (JSON Web Token) based authentication
- Swagger integration for API documentation
- Password hashing for secure user credentials

## 🛠️ Technologies Used
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server (or any compatible database)
- JWT Authentication
- Swagger (Swashbuckle)
- C#

## 🔐 Authentication
JWT tokens are used to authenticate users and protect API endpoints. Upon successful login, a token is generated and must be included in the `Authorization` header for subsequent requests.

## 🔑 Password Hashing
Passwords are securely stored using hashing algorithms. This ensures that even if the database is compromised, the original passwords remain protected. Recommended hashing methods include:
- SHA-256 (basic hashing)
- bcrypt (slow and secure)
- scrypt (memory-intensive)
- Argon2 (modern and configurable)

## 📄 API Documentation
Swagger is integrated to provide interactive documentation for all API endpoints. You can test endpoints directly from the Swagger UI.

## 📁 Folder Structure
- `Controllers/` - API controllers
- `Models/` - Entity models
- `Dtos/` - Data Transfer Objects
- `Data/` - Database context
- `Startup.cs` - Configuration and middleware setup

## 🧪 Getting Started
1. Clone the repository
2. Configure the database connection string in `appsettings.json`
3. Run migrations to create the database
4. Start the application and navigate to `/swagger` for API documentation
