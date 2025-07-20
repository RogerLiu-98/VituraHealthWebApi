using System.ComponentModel.DataAnnotations;

namespace VituraHealthWebApi.DataAccess.Entities;

public class PrescriptionEntity
{
    [Key]
    public int Id { get; set; }

    // Foreign key to PatientEntity
    [Required]
    public int PatientId { get; set; }

    [Required]
    [StringLength(200)]
    public required string DrugName { get; set; }

    [Required]
    [StringLength(50)]
    public required string Dosage { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DatePrescribed { get; set; }

    // Navigation property for related patient
    public virtual PatientEntity? Patient { get; set; }
}
