using System;
using VituraHealthWebApi.DataAccess.Entities;

namespace VituraHealthWebApi.DataAccess.Repositories.Interfaces;

public interface IPatientRepository
{
    Task<IList<PatientEntity>> GetAllPatientsAsync();
}
