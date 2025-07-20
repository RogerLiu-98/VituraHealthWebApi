using System;
using VituraHealthWebApi.Models;

namespace VituraHealthWebApi.Services.Interfaces;

public interface IPrescriptionService
{
    Task<IList<Prescription>> GetAllPrescriptionsAsync();
    Task<Prescription> CreatePrescriptionAsync(CreatePrescriptionRequest prescription, DateTime datePrescribed);
}
