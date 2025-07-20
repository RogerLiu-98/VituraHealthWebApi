using System.Text.Json.Serialization;

namespace VituraHealthWebApi.Models;

public class Prescription
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("patientId")]
    public int PatientId { get; set; }

    [JsonPropertyName("drugName")]
    public required string DrugName { get; set; }

    [JsonPropertyName("dosage")]
    public required string Dosage { get; set; }

    [JsonPropertyName("datePrescribed")]
    public DateTime DatePrescribed { get; set; }
}
