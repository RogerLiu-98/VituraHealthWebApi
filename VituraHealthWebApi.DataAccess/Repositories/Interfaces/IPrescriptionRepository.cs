using System;
using VituraHealthWebApi.DataAccess.Entities;

namespace VituraHealthWebApi.DataAccess.Repositories.Interfaces;

public interface IPrescriptionRepository
{
    Task<IList<PrescriptionEntity>> GetAllPrescriptionsAsync();
    Task<PrescriptionEntity> AddPrescriptionAsync(PrescriptionEntity prescription);
}
