# VituraHealthWebApi.Tests

This project contains comprehensive unit tests for the VituraHealthWebApi business service layer.

## Test Coverage

### PatientService Tests
- `GetAllPatientsAsync()` method with various scenarios:
  - When patients exist in the repository
  - When no patients exist 
  - When repository returns null
  - When repository throws exceptions
- Constructor validation tests

### PrescriptionService Tests  
- `CreatePrescriptionAsync()` method testing:
  - Valid prescription creation
  - Exception handling during creation
- `GetAllPrescriptionsAsync()` method with various scenarios:
  - When prescriptions exist in the repository
  - When no prescriptions exist
  - When repository returns null
  - When repository throws exceptions
- Constructor validation tests

### DataLoadingService Tests
- `LoadDataAsync()` method testing:
  - Loading valid patient and prescription data from JSON files
  - Handling missing data files
  - Handling empty data files
  - Invalid JSON format handling
  - Missing DataPath configuration
- Constructor validation tests

### Integration Tests
- End-to-end workflow testing:
  - Loading initial data
  - Creating new prescriptions
  - Retrieving updated data
- Multiple prescriptions for same patient testing

## Test Technologies Used

- **xUnit**: Primary testing framework
- **Moq**: Mocking framework for dependencies
- **FluentAssertions**: Enhanced assertion library for better test readability
- **Entity Framework In-Memory**: In-memory database for testing data access
- **Temporary file system**: For testing file-based operations

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "ClassName=PatientServiceTests"
```

## Test Structure

Tests follow the **Arrange-Act-Assert** pattern and include:
- Comprehensive edge case coverage
- Proper exception testing
- Mocking of external dependencies
- Cleanup of test resources (IDisposable pattern)
- Integration testing with real EF Core context
