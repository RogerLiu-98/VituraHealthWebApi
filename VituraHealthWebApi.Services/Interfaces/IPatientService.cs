using System;
using VituraHealthWebApi.Models;

namespace VituraHealthWebApi.Services.Interfaces;

public interface IPatientService
{
    Task<IList<Patient>> GetAllPatientsAsync();
}
