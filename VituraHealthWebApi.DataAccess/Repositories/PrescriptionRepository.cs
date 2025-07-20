using System;
using Microsoft.EntityFrameworkCore;
using VituraHealthWebApi.DataAccess.Contexts;
using VituraHealthWebApi.DataAccess.Entities;
using VituraHealthWebApi.DataAccess.Repositories.Interfaces;

namespace VituraHealthWebApi.DataAccess.Repositories;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly VituraHealthDbContext _context;

    public PrescriptionRepository(VituraHealthDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PrescriptionEntity> AddPrescriptionAsync(PrescriptionEntity prescription)
    {
        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();
        return prescription;
    }

    public async Task<IList<PrescriptionEntity>> GetAllPrescriptionsAsync()
    {
        return await _context.Prescriptions.ToListAsync();
    }
}
