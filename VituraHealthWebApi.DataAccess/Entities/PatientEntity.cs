using System.ComponentModel.DataAnnotations;

namespace VituraHealthWebApi.DataAccess.Entities;

public class PatientEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public required string FullName { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    // Navigation property for related prescriptions
    public virtual ICollection<PrescriptionEntity>? Prescriptions { get; set; }
}
