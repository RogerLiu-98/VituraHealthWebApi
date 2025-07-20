using System;
using Microsoft.EntityFrameworkCore;
using VituraHealthWebApi.DataAccess.Contexts;
using VituraHealthWebApi.DataAccess.Entities;
using VituraHealthWebApi.DataAccess.Repositories.Interfaces;

namespace VituraHealthWebApi.DataAccess.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly VituraHealthDbContext _context;

    public PatientRepository(VituraHealthDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IList<PatientEntity>> GetAllPatientsAsync()
    {
        return await _context.Patients.ToListAsync();
    }
}
