using System;
using Microsoft.Extensions.Logging;
using VituraHealthWebApi.DataAccess.Entities;
using VituraHealthWebApi.DataAccess.Repositories.Interfaces;
using VituraHealthWebApi.Models;
using VituraHealthWebApi.Services.Interfaces;

namespace VituraHealthWebApi.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly ILogger<PrescriptionService> _logger;

    public PrescriptionService(IPrescriptionRepository prescriptionRepository, ILogger<PrescriptionService> logger)
    {
        _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Prescription> CreatePrescriptionAsync(CreatePrescriptionRequest addPrescriptionRequest)
    {
        try
        {
            _logger.LogInformation("Adding a new prescription for patient {PatientId}.", addPrescriptionRequest.PatientId);

            var prescriptionEntity = new PrescriptionEntity
            {
                PatientId = addPrescriptionRequest.PatientId,
                DrugName = addPrescriptionRequest.DrugName,
                Dosage = addPrescriptionRequest.Dosage,
                DatePrescribed = addPrescriptionRequest.DatePrescribed
            };

            prescriptionEntity = await _prescriptionRepository.AddPrescriptionAsync(prescriptionEntity);
            _logger.LogInformation("Successfully added prescription for patient {PatientId}.", addPrescriptionRequest.PatientId);

            // Map PrescriptionEntity to Prescription model
            var prescription = new Prescription
            {
                Id = prescriptionEntity.Id,
                PatientId = prescriptionEntity.PatientId,
                DrugName = prescriptionEntity.DrugName,
                Dosage = prescriptionEntity.Dosage,
                DatePrescribed = prescriptionEntity.DatePrescribed
            };
            return prescription;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding a prescription for patient {PatientId}.", addPrescriptionRequest.PatientId);
            throw; // Re-throw the exception to be handled by the caller
        }
    }

    public async Task<IList<Prescription>> GetAllPrescriptionsAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all prescriptions.");
            var prescriptionEntities = await _prescriptionRepository.GetAllPrescriptionsAsync();
            if (prescriptionEntities == null || !prescriptionEntities.Any())
            {
                _logger.LogInformation("No prescriptions found.");
                return new List<Prescription>();
            }

            // Map PrescriptionEntity to Prescription model
            var prescriptions = prescriptionEntities.Select(pe => new Prescription
            {
                Id = pe.Id,
                PatientId = pe.PatientId,
                DrugName = pe.DrugName,
                Dosage = pe.Dosage,
                DatePrescribed = pe.DatePrescribed
            }).ToList();
            
            _logger.LogInformation("Successfully retrieved {Count} prescriptions.", prescriptions.Count);
            return prescriptions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving all prescriptions.");
            throw; // Re-throw the exception to be handled by the caller
        }
    }
}
