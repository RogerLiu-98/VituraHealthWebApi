using System;
using Microsoft.Extensions.Logging;
using VituraHealthWebApi.DataAccess.Repositories.Interfaces;
using VituraHealthWebApi.Models;
using VituraHealthWebApi.Services.Interfaces;

namespace VituraHealthWebApi.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public async Task<IList<Patient>> GetAllPatientsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all patients from the repository.");
            var patientEntities = await _patientRepository.GetAllPatientsAsync();
            if (patientEntities == null || !patientEntities.Any())
            {
                _logger.LogInformation("No patients found.");
                return new List<Patient>();
            }
            
            // Map entities to models
            var patients = patientEntities.Select(p => new Patient
            {
                Id = p.Id,
                FullName = p.FullName,
                DateOfBirth = p.DateOfBirth
            }).ToList();
            
            _logger.LogInformation($"Successfully fetched {patients.Count} patients.");
            return patients;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching patients.");
            throw; // Re-throw the exception to be handled by the caller
        }
    }
}
