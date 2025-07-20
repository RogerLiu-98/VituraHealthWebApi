using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VituraHealthWebApi.DataAccess.Entities;
using VituraHealthWebApi.DataAccess.Repositories.Interfaces;
using VituraHealthWebApi.Models;
using VituraHealthWebApi.Services;

namespace VituraHealthWebApi.Tests.Services;

public class PrescriptionServiceTests
{
    private readonly Mock<IPrescriptionRepository> _mockPrescriptionRepository;
    private readonly Mock<ILogger<PrescriptionService>> _mockLogger;
    private readonly PrescriptionService _prescriptionService;

    public PrescriptionServiceTests()
    {
        _mockPrescriptionRepository = new Mock<IPrescriptionRepository>();
        _mockLogger = new Mock<ILogger<PrescriptionService>>();
        _prescriptionService = new PrescriptionService(_mockPrescriptionRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreatePrescriptionAsync_WithValidRequest_ReturnsCreatedPrescription()
    {
        // Arrange
        var request = new CreatePrescriptionRequest
        {
            PatientId = 1,
            DrugName = "Aspirin",
            Dosage = "100mg",
            DatePrescribed = new DateTime(2025, 7, 20)
        };

        var savedEntity = new PrescriptionEntity
        {
            Id = 123,
            PatientId = 1,
            DrugName = "Aspirin",
            Dosage = "100mg",
            DatePrescribed = new DateTime(2025, 7, 20)
        };

        _mockPrescriptionRepository
            .Setup(r => r.AddPrescriptionAsync(It.IsAny<PrescriptionEntity>()))
            .ReturnsAsync(savedEntity);

        // Act
        var result = await _prescriptionService.CreatePrescriptionAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(123);
        result.PatientId.Should().Be(1);
        result.DrugName.Should().Be("Aspirin");
        result.Dosage.Should().Be("100mg");
        result.DatePrescribed.Should().Be(new DateTime(2025, 7, 20));

        _mockPrescriptionRepository.Verify(
            r => r.AddPrescriptionAsync(It.Is<PrescriptionEntity>(e =>
                e.PatientId == 1 &&
                e.DrugName == "Aspirin" &&
                e.Dosage == "100mg" &&
                e.DatePrescribed == new DateTime(2025, 7, 20))),
            Times.Once);
    }

    [Fact]
    public async Task CreatePrescriptionAsync_WhenRepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var request = new CreatePrescriptionRequest
        {
            PatientId = 1,
            DrugName = "Aspirin",
            Dosage = "100mg",
            DatePrescribed = DateTime.UtcNow
        };

        var expectedException = new InvalidOperationException("Database error");
        _mockPrescriptionRepository
            .Setup(r => r.AddPrescriptionAsync(It.IsAny<PrescriptionEntity>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _prescriptionService.CreatePrescriptionAsync(request));

        exception.Should().Be(expectedException);
    }

    [Fact]
    public async Task GetAllPrescriptionsAsync_WhenPrescriptionsExist_ReturnsPrescriptionList()
    {
        // Arrange
        var prescriptionEntities = new List<PrescriptionEntity>
        {
            new PrescriptionEntity 
            { 
                Id = 1, 
                PatientId = 10, 
                DrugName = "Aspirin", 
                Dosage = "100mg", 
                DatePrescribed = new DateTime(2025, 7, 20) 
            },
            new PrescriptionEntity 
            { 
                Id = 2, 
                PatientId = 20, 
                DrugName = "Ibuprofen", 
                Dosage = "200mg", 
                DatePrescribed = new DateTime(2025, 7, 21) 
            }
        };

        _mockPrescriptionRepository
            .Setup(r => r.GetAllPrescriptionsAsync())
            .ReturnsAsync(prescriptionEntities);

        // Act
        var result = await _prescriptionService.GetAllPrescriptionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        
        result[0].Id.Should().Be(1);
        result[0].PatientId.Should().Be(10);
        result[0].DrugName.Should().Be("Aspirin");
        result[0].Dosage.Should().Be("100mg");
        result[0].DatePrescribed.Should().Be(new DateTime(2025, 7, 20));
        
        result[1].Id.Should().Be(2);
        result[1].PatientId.Should().Be(20);
        result[1].DrugName.Should().Be("Ibuprofen");
        result[1].Dosage.Should().Be("200mg");
        result[1].DatePrescribed.Should().Be(new DateTime(2025, 7, 21));

        _mockPrescriptionRepository.Verify(r => r.GetAllPrescriptionsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPrescriptionsAsync_WhenNoPrescriptionsExist_ReturnsEmptyList()
    {
        // Arrange
        _mockPrescriptionRepository
            .Setup(r => r.GetAllPrescriptionsAsync())
            .ReturnsAsync(new List<PrescriptionEntity>());

        // Act
        var result = await _prescriptionService.GetAllPrescriptionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockPrescriptionRepository.Verify(r => r.GetAllPrescriptionsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPrescriptionsAsync_WhenRepositoryReturnsNull_ReturnsEmptyList()
    {
        // Arrange
        _mockPrescriptionRepository
            .Setup(r => r.GetAllPrescriptionsAsync())
            .ReturnsAsync((IList<PrescriptionEntity>)null!);

        // Act
        var result = await _prescriptionService.GetAllPrescriptionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockPrescriptionRepository.Verify(r => r.GetAllPrescriptionsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPrescriptionsAsync_WhenRepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Database error");
        _mockPrescriptionRepository
            .Setup(r => r.GetAllPrescriptionsAsync())
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _prescriptionService.GetAllPrescriptionsAsync());

        exception.Should().Be(expectedException);
        _mockPrescriptionRepository.Verify(r => r.GetAllPrescriptionsAsync(), Times.Once);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new PrescriptionService(null!, _mockLogger.Object));

        exception.ParamName.Should().Be("prescriptionRepository");
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(
            () => new PrescriptionService(_mockPrescriptionRepository.Object, null!));

        exception.ParamName.Should().Be("logger");
    }
}
