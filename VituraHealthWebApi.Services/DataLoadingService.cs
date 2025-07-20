using System.Text.Json;
using Microsoft.Extensions.Configuration;
using VituraHealthWebApi.DataAccess.Contexts;
using VituraHealthWebApi.DataAccess.Entities;
using VituraHealthWebApi.Models;
using VituraHealthWebApi.Services.Interfaces;

namespace VituraHealthWebApi.Services;

public class DataLoadingService : IDataLoadingService
{
    private readonly VituraHealthDbContext _context;
    private readonly string _dataPath;

    public DataLoadingService(VituraHealthDbContext context, IConfiguration configuration)
    {
        _context = context;
        _dataPath = configuration["DataPath"] ?? throw new InvalidOperationException("DataPath not configured");
    }

    public async Task LoadDataAsync()
    {
        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Load and seed patients
            await LoadPatientsAsync();

            // Load and seed prescriptions
            await LoadPrescriptionsAsync();

            // Save all changes
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to seed data from {_dataPath}", ex);
        }
    }

    private async Task LoadPatientsAsync()
    {
        var patientsPath = Path.Combine(_dataPath, "patients.json");
        if (!File.Exists(patientsPath)) return;

        var patientsJson = await File.ReadAllTextAsync(patientsPath);
        var patients = JsonSerializer.Deserialize<List<Patient>>(patientsJson);

        if (patients?.Any() == true)
        {
            // Map patient data to entities if necessary
            var patientEntities = patients.Select(p => new PatientEntity
            {
                Id = p.Id,
                FullName = p.FullName,
                DateOfBirth = p.DateOfBirth
            }).ToList();
            await _context.Patients.AddRangeAsync(patientEntities);
        }
    }

    private async Task LoadPrescriptionsAsync()
    {
        var prescriptionsPath = Path.Combine(_dataPath, "prescriptions.json");
        if (!File.Exists(prescriptionsPath)) return;

        var prescriptionsJson = await File.ReadAllTextAsync(prescriptionsPath);
        var prescriptions = JsonSerializer.Deserialize<List<Prescription>>(prescriptionsJson);

        if (prescriptions?.Any() == true)
        {
            // Map prescription data to entities if necessary
            var prescriptionEntities = prescriptions.Select(p => new PrescriptionEntity
            {
                Id = p.Id,
                PatientId = p.PatientId,
                DrugName = p.DrugName,
                Dosage = p.Dosage,
                DatePrescribed = p.DatePrescribed
            }).ToList();
            await _context.Prescriptions.AddRangeAsync(prescriptionEntities);
        }
    }
}
