# VituraHealth Web API

A RESTful Web API for managing patients and prescriptions, built with ASP.NET Core and Entity Framework Core. The API uses an in-memory database loaded from JSON data files for demonstration purposes.

## 🚀 How to Run the Project

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio Code or Visual Studio

### Running the Application

1. **Clone and navigate to the project:**
   ```bash
   git clone <repository-url>
   cd VituraHealthWebApi
   ```

2. **Build the solution:**
   ```bash
   dotnet build
   ```

3. **Run the Web API:**
   ```bash
   cd VituraHealthWebApi
   dotnet run
   ```

4. **Access the API:**
   - **Swagger UI**: https://localhost:7182/swagger/index.html
   - **HTTP**: http://localhost:5202
   - **HTTPS**: https://localhost:7182

5. **Check startup logs:**
   ```
   info: Starting data loading...
   info: Data loading completed successfully.
   ```

### Running Tests

```bash
cd VituraHealthWebApi.Tests
dotnet test
```

## 📋 Available Endpoints

### Patients
- `GET /api/patients` - Get all patients

### Prescriptions
- `GET /api/prescriptions` - Get all prescriptions
- `POST /api/prescriptions` - Create a new prescription

### Sample Request (Create Prescription)
```json
POST /api/prescriptions
{
    "patientId": 1,
    "drugName": "Aspirin",
    "dosage": "100mg daily"
}
```

## 🏗️ Project Structure

```
VituraHealthWebApi/
├── VituraHealthWebApi/              # Main Web API project
│   ├── Controllers/                 # API controllers
│   ├── Properties/                  # Launch settings
│   └── Program.cs                   # Application entry point
├── VituraHealthWebApi.DataAccess/   # Data access layer
│   ├── Contexts/                    # Entity Framework DbContext
│   ├── Entities/                    # Database entities
│   └── Data/                        # JSON data files
├── VituraHealthWebApi.Models/       # Business Data models
│   └── RequestModels/               # API request models
├── VituraHealthWebApi.Services/     # Business logic layer
└── VituraHealthWebApi.Tests/        # Unit tests
    └── Services/                    # Service layer tests
```

## 🔧 Architecture Overview

### In-Memory Database Approach
1. **Startup**: Data is loaded once from JSON files into Entity Framework in-memory database
2. **Runtime**: All queries execute against the in-memory database (no file I/O)
3. **Performance**: Fast data access with full Entity Framework features
4. **Data Seeding**: Protected by application startup to ensure single load

### Data Flow
```
Application Startup
    ↓
DataLoadingService loads JSON files
    ↓
Parse JSON → Insert into InMemory Database
    ↓
Application Ready (Data in Memory)
    ↓
API Requests → Business Services → Entity Framework → InMemory DB
```

## ⚠️ Assumptions and Shortcuts

### Data Model Assumptions
- **Patient-Prescription Relationship**: One-to-many relationship (one patient can have multiple prescriptions)
- **Required Fields**: All fields in both Patient and Prescription entities are marked as required
- **Date Format**: All dates are stored and returned in `yyyy-MM-dd` format (date only, no time)
- **Data Validation**: Basic validation on required fields, no complex business rules implemented

### Implementation Shortcuts
- **Static Data**: Data is loaded from static JSON files at startup
- **No Persistence**: Changes made via API are lost on restart (in-memory only)
- **Simplified Error Handling**: Basic exception handling without detailed error categorization
- **No Authentication/Authorization**: API endpoints are publicly accessible
- **No Pagination**: All endpoints return complete datasets
- **Minimal Validation**: Basic model validation without complex business rule validation

## 🚧 Known Limitations

### Technical Limitations
- **No AutoMapper**: Manual mapping between business models and database entities
  - *Impact*: More boilerplate code for object mapping
  - *Future Enhancement*: Implement AutoMapper for cleaner mapping logic

- **Basic Logging**: Simple logging without correlation IDs or structured logging
  - *Current*: Basic log messages with minimal context
  - *Missing*: Correlation IDs, request tracking, performance metrics
  - *Future Enhancement*: Implement structured logging with Serilog and correlation IDs

- **In-Memory Database**: Data doesn't persist between application restarts
  - *Impact*: Any POST/PUT/DELETE operations are temporary
  - *Future Enhancement*: Replace with persistent database (SQL Server, PostgreSQL)

### API Limitations
- **No Versioning**: API versioning not implemented
- **Limited Error Responses**: Basic error responses without detailed error codes
- **No Rate Limiting**: No protection against API abuse
- **No Health Checks**: No health check endpoints for monitoring

## 🧪 Testing

The project includes comprehensive unit tests covering:
- **Service Layer**: Business logic testing with mocking
- **Data Loading**: JSON file processing and error handling
- **Integration Tests**: End-to-end service interaction testing

**Test Coverage**: 23 tests with full service layer coverage

## 📦 Dependencies

### Core Dependencies
- `Microsoft.EntityFrameworkCore` - ORM framework
- `Microsoft.EntityFrameworkCore.InMemory` - In-memory database provider
- `System.Text.Json` - JSON serialization
- `Microsoft.AspNetCore.OpenApi` - Swagger/OpenAPI support

### Testing Dependencies
- `xUnit` - Testing framework
- `Moq` - Mocking framework
- `FluentAssertions` - Better test assertions
- `Microsoft.EntityFrameworkCore.InMemory` - In-memory database for tests

## 🔧 Configuration

### appsettings.json
```json
{
  "DataPath": "../VituraHealthWebApi.DataAccess/data",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

This project demonstrates a clean architecture approach with separation of concerns, comprehensive testing, and modern .NET development practices, while acknowledging areas for future improvement in scalability, persistence, and advanced features.