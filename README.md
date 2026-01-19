# 🚗 Car Sales Management API

<div align="center">
  <img src="https://img.shields.io/badge/.NET%20Core-8.0-blue" alt=".NET Core">
  <img src="https://img.shields.io/badge/Architecture-Clean%20Architecture-green" alt="Architecture">
  <img src="https://img.shields.io/badge/Database-SQL%20Server-orange" alt="Database">
  <img src="https://img.shields.io/badge/Status-Production%20Ready-brightgreen" alt="Status">
</div>



## 📋 Table of Contents

- [🌟 Overview](#-overview)
- [✨ Features](#-features)
- [🏗️ Architecture](#️-architecture)
- [🔧 Tech Stack](#-tech-stack)
- [📦 Installation](#-installation)
- [⚙️ Configuration](#️-configuration)
- [🚀 Getting Started](#-getting-started)
- [📚 API Documentation](#-api-documentation)
- [🔐 Security](#-security)
- [🧪 Testing](#-testing)
- [📝 Project Structure](#-project-structure)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)

---

## 🌟 Overview

The **Car Sales Management API** is a robust, scalable backend system built with .NET Core 8.0 that provides comprehensive car model management and sales commission calculation capabilities. This API follows Clean Architecture principles and implements enterprise-grade patterns for maintainability and scalability.

### 🎯 Business Purpose

- **Car Model Management**: Complete CRUD operations for vehicle inventory
- **Sales Commission Tracking**: Automated commission calculations based on complex business rules
- **Multi-brand Support**: Audi, Jaguar, Land Rover, and Renault
- **Role-based Access**: Dynamic menu generation for different user roles

---

## ✨ Features

### 🚗 Car Model Management
- ✅ **Complete CRUD Operations** - Create, Read, Update, Delete car models
- ✅ **Brand Management** - Support for Audi, Jaguar, Land Rover, Renault
- ✅ **Class Classification** - A-Class, B-Class, C-Class categorization
- ✅ **Rich Text Support** - HTML descriptions and features using CKEditor
- ✅ **Image Management** - Multiple image uploads with 5MB limit
- ✅ **Advanced Validation** - Comprehensive field validation with FluentValidation
- ✅ **Search & Filtering** - Search by model name, code with sorting options

### 💰 Commission System
- ✅ **Brand-wise Fixed Commission** - Different rates per brand
- ✅ **Class-wise Percentage Commission** - Variable percentages by car class
- ✅ **Performance Bonus** - Additional 2% for high performers
- ✅ **Complex Business Logic** - Multi-tiered calculation engine
- ✅ **Historical Tracking** - Previous year sales consideration

### 🔐 Security & Architecture
- ✅ **JWT Authentication** - Token-based security
- ✅ **Role-based Authorization** - Dynamic menu creation
- ✅ **Centralized Error Handling** - Global exception middleware
- ✅ **API Documentation** - Swagger/OpenAPI integration
- ✅ **CORS Support** - Cross-origin resource sharing
- ✅ **Dependency Injection** - IoC container pattern

---

## 🏗️ Architecture

```
📁 CarSalesManagementAPI/
├── 📁 API/                    # Controllers, Middleware
├── 📁 Application/             # Services, DTOs, Validators
├── 📁 Domain/                 # Entities, Interfaces
├── 📁 Infrastructure/           # Data Access, Repositories
└── 📁 wwwroot/               # Static Files
```

### 🎯 Design Patterns
- **Clean Architecture** - Separation of concerns
- **Repository Pattern** - Data access abstraction
- **Service Layer** - Business logic encapsulation
- **DTO Pattern** - Data transfer objects
- **Dependency Injection** - Loose coupling

---

## 🔧 Tech Stack

### 🛠️ Core Technologies
| Technology | Version | Purpose |
|------------|----------|---------|
| **.NET Core** | 8.0 | Framework |
| **C#** | 12.0 | Language |
| **SQL Server** | 2019+ | Database |
| **Dapper** | 2.0+ | ORM Alternative |

### 📦 Key Libraries
| Library | Purpose |
|----------|---------|
| **FluentValidation** | Input Validation |
| **AutoMapper** | Object Mapping |
| **Swashbuckle** | API Documentation |
| **System.Data.SqlClient** | Database Connectivity |

---

## 📦 Installation

### 📋 Prerequisites
- **.NET 8.0 SDK** or later
- **SQL Server** 2019 or later
- **Visual Studio 2022** or VS Code

### 🚀 Setup Steps

1. **📥 Clone the Repository**
   ```bash
   git clone <repository-url>
   cd CarSalesManagement/API
   ```

2. **🗄️ Database Setup**
   ```sql
   -- Create database
   CREATE DATABASE CarSalesManagementDB;
   ```

3. **⚙️ Configuration**
   ```bash
   # Copy configuration template
   cp appsettings.json appsettings.Development.json
   
   # Update connection string
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=CarSalesManagementDB;Integrated Security=true;TrustServerCertificate=true;"
   }
   ```

4. **📦 Restore Dependencies**
   ```bash
   dotnet restore
   ```

5. **🏗️ Build the Project**
   ```bash
   dotnet build
   ```

6. **🚀 Run the Application**
   ```bash
   dotnet run
   ```

---

## ⚙️ Configuration

### 📄 Application Settings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CarSalesManagementDB;Integrated Security=true;"
  },
  "FileUpload": {
    "MaxFileSize": 5242880,
    "AllowedExtensions": [ ".jpg", ".jpeg", ".png", ".gif" ],
    "UploadPath": "wwwroot/uploads/carmodels"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 🔐 Security Configuration
- **JWT Settings**: Configure token expiration and secret keys
- **CORS Policy**: Whitelist allowed origins
- **File Upload**: Restrict file types and sizes

---

## 🚀 Getting Started

### 🏃‍♂️ Quick Start

1. **🌐 Access API**
   - **Base URL**: `https://localhost:5001`
   - **Swagger UI**: `https://localhost:5001/swagger`

2. **🔐 Authentication**
   ```http
   POST /api/auth/login
   Content-Type: application/json
   
   {
     "username": "admin",
     "password": "password"
   }
   ```

3. **📝 Create Car Model**
   ```http
   POST /api/carmodels
   Authorization: Bearer {token}
   Content-Type: application/json
   
   {
     "brandID": 1,
     "classID": 1,
     "modelName": "Audi A4",
     "modelCode": "AUDIA42024",
     "description": "Luxury sedan",
     "features": "Premium features",
     "price": 45000,
     "dateOfManufacturing": "2024-01-15",
     "isActive": true,
     "sortOrder": 1
   }
   ```

---

## 📚 API Documentation

### 🌐 Swagger Documentation
Visit `https://localhost:5001/swagger` for interactive API documentation.

### 📋 Key Endpoints

#### 🔐 Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | User login |
| POST | `/api/auth/refresh` | Refresh token |

#### 🚗 Car Models
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/carmodels` | Get all car models |
| GET | `/api/carmodels/{id}` | Get car model by ID |
| POST | `/api/carmodels` | Create new car model |
| PUT | `/api/carmodels/{id}` | Update car model |
| DELETE | `/api/carmodels/{id}` | Delete car model |
| GET | `/api/carmodels/brands` | Get all brands |
| GET | `/api/carmodels/classes` | Get all classes |

#### 📊 Commission Reports
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/commission/report/{salesmanId}` | Generate commission report |
| GET | `/api/commission/report/all` | Generate all salesmen reports |

---

## 🔐 Security

### 🛡️ Security Features
- **JWT Authentication** - Stateless token-based auth
- **Password Hashing** - Secure password storage
- **Input Validation** - Comprehensive input sanitization
- **CORS Protection** - Cross-origin request control
- **Rate Limiting** - API abuse prevention
- **SQL Injection Protection** - Parameterized queries

### 🔑 Authentication Flow
1. **Login Request** → Validate credentials
2. **Token Generation** → Create JWT with claims
3. **Token Validation** → Verify on each request
4. **Authorization** → Role-based access control

---

## 🧪 Testing

### 🧪 Unit Tests
```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### 🔍 Integration Tests
```bash
# Run integration tests
dotnet test --filter "Category=Integration"
```

### 📊 Test Coverage
- **Controllers**: 90%+
- **Services**: 85%+
- **Repositories**: 80%+

---

## 📝 Project Structure

```
📁 CarSalesManagementAPI/
├── 📁 API/
│   ├── 📁 Controllers/          # API Controllers
│   │   ├── AuthController.cs
│   │   ├── CarModelsController.cs
│   │   ├── CommissionReportController.cs
│   │   └── MenuController.cs
│   └── 📁 Middleware/          # Custom Middleware
│       └── ErrorHandlingMiddleware.cs
├── 📁 Application/
│   ├── 📁 DTOs/              # Data Transfer Objects
│   ├── 📁 Services/           # Business Logic
│   ├── 📁 Validators/         # Validation Rules
│   └── 📁 Mapping/           # Object Mapping
├── 📁 Domain/
│   ├── 📁 Entities/           # Domain Models
│   └── 📁 Interfaces/         # Repository Contracts
├── 📁 Infrastructure/
│   ├── 📁 Data/              # Data Access
│   └── 📁 Repositories/      # Repository Implementations
└── 📁 wwwroot/              # Static Files
```

---

## 🤝 Contributing

### 📝 Development Guidelines
1. **🌿 Create Feature Branch**
   ```bash
   git checkout -b feature/amazing-feature
   ```

2. **📏 Follow Coding Standards**
   - Use C# conventions
   - Add XML documentation
   - Write unit tests
   - Follow Clean Architecture

3. **📤 Submit Pull Request**
   - Describe changes clearly
   - Include test coverage
   - Update documentation

### 🎯 Code Quality
- **Code Coverage**: Minimum 80%
- **Documentation**: All public APIs documented
- **Performance**: Response time < 200ms
- **Security**: Follow OWASP guidelines

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- **Microsoft** - .NET Core framework
- **Dapper Team** - High-performance micro-ORM
- **FluentValidation** - Validation library
- **AutoMapper** - Object mapping framework

---

<div align="center">
  <p>🚗 <strong>Happy Coding!</strong> 🚗</p>
  <p>Made with ❤️ by Car Sales Management Team</p>
</div>
