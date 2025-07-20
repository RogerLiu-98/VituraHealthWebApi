using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using VituraHealthWebApi.DataAccess.Contexts;
using VituraHealthWebApi.DataAccess.Repositories;
using VituraHealthWebApi.Models;
using VituraHealthWebApi.Services;

namespace VituraHealthWebApi.Tests.Services;

public class ServiceIntegrationTests : IDisposable
{
    private readonly VituraHealthDbContext _context;
    private readonly PatientService _patientService;
    private readonly PrescriptionService _prescriptionService;
    private readonly DataLoadingService _dataLoadingService;
    private readonly string _testDataPath;

    public ServiceIntegrationTests()
    {
        // Create in-memory database
        var options = new DbContextOptionsBuilder<VituraHealthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new VituraHealthDbContext(options);

        // Setup test data path
        _testDataPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDataPath);

        // Setup configuration
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(c => c["DataPath"]).Returns(_testDataPath);

        // Setup loggers
        var patientLogger = new Mock<ILogger<PatientService>>();
        var prescriptionLogger = new Mock<ILogger<PrescriptionService>>();

        // Create repositories
        var patientRepository = new PatientRepository(_context);
        var prescriptionRepository = new PrescriptionRepository(_context);

        // Create services
        _patientService = new PatientService(patientRepository, patientLogger.Object);
        _prescriptionService = new PrescriptionService(prescriptionRepository, prescriptionLogger.Object);
        _dataLoadingService = new DataLoadingService(_context, mockConfiguration.Object);
    }

    [Fact]
    public async Task EndToEndWorkflow_LoadDataCreatePrescriptionAndRetrieve_WorksCorrectly()
    {
        // Arrange - Create test data files
        await CreateTestDataFiles();

        // Act 1 - Load initial data
        await _dataLoadingService.LoadDataAsync();

        // Assert 1 - Verify data was loaded
        var initialPatients = await _patientService.GetAllPatientsAsync();
        var initialPrescriptions = await _prescriptionService.GetAllPrescriptionsAsync();

        initialPatients.Should().HaveCount(2);
        initialPrescriptions.Should().HaveCount(2);

        // Act 2 - Create a new prescription
        var newPrescriptionRequest = new CreatePrescriptionRequest
        {
            PatientId = 1,
            DrugName = "Tylenol",
            Dosage = "500mg",
            DatePrescribed = new DateTime(2025, 7, 22)
        };

        var createdPrescription = await _prescriptionService.CreatePrescriptionAsync(newPrescriptionRequest);

        // Assert 2 - Verify prescription was created
        createdPrescription.Should().NotBeNull();
        createdPrescription.PatientId.Should().Be(1);
        createdPrescription.DrugName.Should().Be("Tylenol");
        createdPrescription.Dosage.Should().Be("500mg");
        createdPrescription.DatePrescribed.Should().Be(new DateTime(2025, 7, 22));

        // Act 3 - Retrieve all prescriptions again
        var allPrescriptions = await _prescriptionService.GetAllPrescriptionsAsync();

        // Assert 3 - Verify we now have 3 prescriptions
        allPrescriptions.Should().HaveCount(3);
        allPrescriptions.Should().Contain(p => p.DrugName == "Tylenol");

        // Act 4 - Verify patients haven't changed
        var finalPatients = await _patientService.GetAllPatientsAsync();

        // Assert 4 - Verify patients count is still the same
        finalPatients.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateMultiplePrescriptionsForSamePatient_AllPrescriptionsAreSaved()
    {
        // Arrange - Create test data files
        await CreateTestDataFiles();
        await _dataLoadingService.LoadDataAsync();

        var prescriptionRequests = new[]
        {
            new CreatePrescriptionRequest 
            { 
                PatientId = 1, 
                DrugName = "Medicine A", 
                Dosage = "100mg", 
                DatePrescribed = DateTime.UtcNow 
            },
            new CreatePrescriptionRequest 
            { 
                PatientId = 1, 
                DrugName = "Medicine B", 
                Dosage = "200mg", 
                DatePrescribed = DateTime.UtcNow.AddDays(1) 
            },
            new CreatePrescriptionRequest 
            { 
                PatientId = 1, 
                DrugName = "Medicine C", 
                Dosage = "300mg", 
                DatePrescribed = DateTime.UtcNow.AddDays(2) 
            }
        };

        // Act
        var createdPrescriptions = new List<Prescription>();
        foreach (var request in prescriptionRequests)
        {
            var prescription = await _prescriptionService.CreatePrescriptionAsync(request);
            createdPrescriptions.Add(prescription);
        }

        var allPrescriptions = await _prescriptionService.GetAllPrescriptionsAsync();

        // Assert
        createdPrescriptions.Should().HaveCount(3);
        allPrescriptions.Should().HaveCount(5); // 2 from initial data + 3 new ones

        var patient1Prescriptions = allPrescriptions.Where(p => p.PatientId == 1).ToList();
        patient1Prescriptions.Should().HaveCount(4); // 1 from initial data + 3 new ones

        patient1Prescriptions.Should().Contain(p => p.DrugName == "Medicine A");
        patient1Prescriptions.Should().Contain(p => p.DrugName == "Medicine B");
        patient1Prescriptions.Should().Contain(p => p.DrugName == "Medicine C");
    }

    private async Task CreateTestDataFiles()
    {
        var patients = new[]
        {
            new { id = 1, fullName = "John Doe", dateOfBirth = new DateTime(1990, 1, 1) },
            new { id = 2, fullName = "Jane Smith", dateOfBirth = new DateTime(1985, 5, 15) }
        };

        var prescriptions = new[]
        {
            new { id = 1, patientId = 1, drugName = "Aspirin", dosage = "100mg", datePrescribed = new DateTime(2025, 7, 20) },
            new { id = 2, patientId = 2, drugName = "Ibuprofen", dosage = "200mg", datePrescribed = new DateTime(2025, 7, 21) }
        };

        var patientsJson = System.Text.Json.JsonSerializer.Serialize(patients);
        var prescriptionsJson = System.Text.Json.JsonSerializer.Serialize(prescriptions);

        var patientsPath = Path.Combine(_testDataPath, "patients.json");
        var prescriptionsPath = Path.Combine(_testDataPath, "prescriptions.json");

        await File.WriteAllTextAsync(patientsPath, patientsJson);
        await File.WriteAllTextAsync(prescriptionsPath, prescriptionsJson);
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
