using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using VituraHealthWebApi.DataAccess.Contexts;
using VituraHealthWebApi.DataAccess.Entities;
using VituraHealthWebApi.Services;
using System.Text.Json;

namespace VituraHealthWebApi.Tests.Services;

public class DataLoadingServiceTests : IDisposable
{
    private readonly VituraHealthDbContext _context;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly DataLoadingService _dataLoadingService;
    private readonly string _testDataPath;

    public DataLoadingServiceTests()
    {
        // Create in-memory database
        var options = new DbContextOptionsBuilder<VituraHealthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new VituraHealthDbContext(options);

        // Setup test data path
        _testDataPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDataPath);

        // Setup configuration mock
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["DataPath"]).Returns(_testDataPath);

        _dataLoadingService = new DataLoadingService(_context, _mockConfiguration.Object);
    }

    [Fact]
    public async Task LoadDataAsync_WithValidData_LoadsPatientsAndPrescriptions()
    {
        // Arrange
        await CreateTestDataFiles();

        // Act
        await _dataLoadingService.LoadDataAsync();

        // Assert
        var patients = await _context.Patients.ToListAsync();
        var prescriptions = await _context.Prescriptions.ToListAsync();

        patients.Should().HaveCount(2);
        patients[0].Id.Should().Be(1);
        patients[0].FullName.Should().Be("John Doe");
        patients[0].DateOfBirth.Should().Be(new DateTime(1990, 1, 1));

        patients[1].Id.Should().Be(2);
        patients[1].FullName.Should().Be("Jane Smith");
        patients[1].DateOfBirth.Should().Be(new DateTime(1985, 5, 15));

        prescriptions.Should().HaveCount(2);
        prescriptions[0].Id.Should().Be(1);
        prescriptions[0].PatientId.Should().Be(1);
        prescriptions[0].DrugName.Should().Be("Aspirin");
        prescriptions[0].Dosage.Should().Be("100mg");

        prescriptions[1].Id.Should().Be(2);
        prescriptions[1].PatientId.Should().Be(2);
        prescriptions[1].DrugName.Should().Be("Ibuprofen");
        prescriptions[1].Dosage.Should().Be("200mg");
    }

    [Fact]
    public async Task LoadDataAsync_WithMissingPatientsFile_LoadsOnlyPrescriptions()
    {
        // Arrange
        await CreateTestPrescriptionsFile();
        // Don't create patients file

        // Act
        await _dataLoadingService.LoadDataAsync();

        // Assert
        var patients = await _context.Patients.ToListAsync();
        var prescriptions = await _context.Prescriptions.ToListAsync();

        patients.Should().BeEmpty();
        prescriptions.Should().HaveCount(2);
    }

    [Fact]
    public async Task LoadDataAsync_WithMissingPrescriptionsFile_LoadsOnlyPatients()
    {
        // Arrange
        await CreateTestPatientsFile();
        // Don't create prescriptions file

        // Act
        await _dataLoadingService.LoadDataAsync();

        // Assert
        var patients = await _context.Patients.ToListAsync();
        var prescriptions = await _context.Prescriptions.ToListAsync();

        patients.Should().HaveCount(2);
        prescriptions.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadDataAsync_WithEmptyDataFiles_DoesNotLoadAnyData()
    {
        // Arrange
        await CreateEmptyDataFiles();

        // Act
        await _dataLoadingService.LoadDataAsync();

        // Assert
        var patients = await _context.Patients.ToListAsync();
        var prescriptions = await _context.Prescriptions.ToListAsync();

        patients.Should().BeEmpty();
        prescriptions.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithMissingDataPath_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["DataPath"]).Returns((string)null!);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => new DataLoadingService(_context, _mockConfiguration.Object));

        exception.Message.Should().Be("DataPath not configured");
    }

    [Fact]
    public async Task LoadDataAsync_WithInvalidJsonFormat_ThrowsInvalidOperationException()
    {
        // Arrange
        var patientsPath = Path.Combine(_testDataPath, "patients.json");
        await File.WriteAllTextAsync(patientsPath, "invalid json content");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _dataLoadingService.LoadDataAsync());

        exception.Message.Should().StartWith($"Failed to seed data from {_testDataPath}");
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullConfiguration_ThrowsNullReferenceException()
    {
        // Act & Assert
        var exception = Assert.Throws<NullReferenceException>(
            () => new DataLoadingService(_context, null!));

        // The exception will be thrown when accessing configuration["DataPath"]
    }

    private async Task CreateTestDataFiles()
    {
        await CreateTestPatientsFile();
        await CreateTestPrescriptionsFile();
    }

    private async Task CreateTestPatientsFile()
    {
        var patients = new[]
        {
            new { id = 1, fullName = "John Doe", dateOfBirth = new DateTime(1990, 1, 1) },
            new { id = 2, fullName = "Jane Smith", dateOfBirth = new DateTime(1985, 5, 15) }
        };

        var patientsJson = JsonSerializer.Serialize(patients);
        var patientsPath = Path.Combine(_testDataPath, "patients.json");
        await File.WriteAllTextAsync(patientsPath, patientsJson);
    }

    private async Task CreateTestPrescriptionsFile()
    {
        var prescriptions = new[]
        {
            new { id = 1, patientId = 1, drugName = "Aspirin", dosage = "100mg", datePrescribed = new DateTime(2025, 7, 20) },
            new { id = 2, patientId = 2, drugName = "Ibuprofen", dosage = "200mg", datePrescribed = new DateTime(2025, 7, 21) }
        };

        var prescriptionsJson = JsonSerializer.Serialize(prescriptions);
        var prescriptionsPath = Path.Combine(_testDataPath, "prescriptions.json");
        await File.WriteAllTextAsync(prescriptionsPath, prescriptionsJson);
    }

    private async Task CreateEmptyDataFiles()
    {
        var patientsPath = Path.Combine(_testDataPath, "patients.json");
        var prescriptionsPath = Path.Combine(_testDataPath, "prescriptions.json");

        await File.WriteAllTextAsync(patientsPath, "[]");
        await File.WriteAllTextAsync(prescriptionsPath, "[]");
    }

    public void Dispose()
    {
        _context.Dispose();
        
        // Clean up test data directory
        if (Directory.Exists(_testDataPath))
        {
            Directory.Delete(_testDataPath, true);
        }
    }
}
