using Microsoft.EntityFrameworkCore;
using VituraHealthWebApi.DataAccess.Entities;

namespace VituraHealthWebApi.DataAccess.Contexts;

public class VituraHealthDbContext : DbContext
{
    public VituraHealthDbContext(DbContextOptions<VituraHealthDbContext> options) : base(options)
    {
    }

    public DbSet<PatientEntity> Patients { get; set; }
    public DbSet<PrescriptionEntity> Prescriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Patient entity
        modelBuilder.Entity<PatientEntity>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.FullName).IsRequired().HasMaxLength(200);
            entity.Property(p => p.DateOfBirth).IsRequired();
            
            // Configure one-to-many relationship: Patient -> Prescriptions
            entity.HasMany(p => p.Prescriptions)
                .WithOne(pr => pr.Patient)
                .HasForeignKey(pr => pr.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Prescription entity
        modelBuilder.Entity<PrescriptionEntity>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.DrugName).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Dosage).IsRequired().HasMaxLength(50);
            entity.Property(p => p.DatePrescribed).IsRequired();
            entity.Property(p => p.PatientId).IsRequired();

            // Create index for foreign key to improve query performance
            entity.HasIndex(p => p.PatientId);
        });
    }
}
