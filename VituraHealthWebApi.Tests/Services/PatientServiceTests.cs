using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VituraHealthWebApi.DataAccess.Entities;
using VituraHealthWebApi.DataAccess.Repositories.Interfaces;
using VituraHealthWebApi.Models;
using VituraHealthWebApi.Services;

namespace VituraHealthWebApi.Tests.Services;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _mockPatientRepository;
    private readonly Mock<ILogger<PatientService>> _mockLogger;
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _mockPatientRepository = new Mock<IPatientRepository>();
        _mockLogger = new Mock<ILogger<PatientService>>();
        _patientService = new PatientService(_mockPatientRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllPatientsAsync_WhenPatientsExist_ReturnsPatientList()
    {
        // Arrange
        var patientEntities = new List<PatientEntity>
        {
            new PatientEntity { Id = 1, FullName = "John Doe", DateOfBirth = new DateTime(1990, 1, 1) },
            new PatientEntity { Id = 2, FullName = "Jane Smith", DateOfBirth = new DateTime(1985, 5, 15) }
        };

        _mockPatientRepository
            .Setup(r => r.GetAllPatientsAsync())
            .ReturnsAsync(patientEntities);

        // Act
        var result = await _patientService.GetAllPatientsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        
        result[0].Id.Should().Be(1);
        result[0].FullName.Should().Be("John Doe");
        result[0].DateOfBirth.Should().Be(new DateTime(1990, 1, 1));
        
        result[1].Id.Should().Be(2);
        result[1].FullName.Should().Be("Jane Smith");
        result[1].DateOfBirth.Should().Be(new DateTime(1985, 5, 15));

        _mockPatientRepository.Verify(r => r.GetAllPatientsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPatientsAsync_WhenNoPatientsExist_ReturnsEmptyList()
    {
        // Arrange
        _mockPatientRepository
            .Setup(r => r.GetAllPatientsAsync())
            .ReturnsAsync(new List<PatientEntity>());

        // Act
        var result = await _patientService.GetAllPatientsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockPatientRepository.Verify(r => r.GetAllPatientsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPatientsAsync_WhenRepositoryReturnsNull_ReturnsEmptyList()
    {
        // Arrange
        _mockPatientRepository
            .Setup(r => r.GetAllPatientsAsync())
            .ReturnsAsync((IList<PatientEntity>)null!);

        // Act
        var result = await _patientService.GetAllPatientsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockPatientRepository.Verify(r => r.GetAllPatientsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPatientsAsync_WhenRepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Database error");
        _mockPatientRepository
            .Setup(r => r.GetAllPatientsAsync())
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _patientService.GetAllPatientsAsync());

        exception.Should().Be(expectedException);
        _mockPatientRepository.Verify(r => r.GetAllPatientsAsync(), Times.Once);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new PatientService(null!, _mockLogger.Object));

        exception.ParamName.Should().Be("patientRepository");
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new PatientService(_mockPatientRepository.Object, null!));

        exception.ParamName.Should().Be("logger");
    }
}
