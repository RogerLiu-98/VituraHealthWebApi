using System;
using System.Text.Json.Serialization;

namespace VituraHealthWebApi.Models;

public class CreatePrescriptionRequest
{
    [JsonPropertyName("patientId")]
    public required int PatientId { get; set; }

    [JsonPropertyName("drugName")]
    public required string DrugName { get; set; }

    [JsonPropertyName("dosage")]
    public required string Dosage { get; set; }

    [JsonPropertyName("datePrescribed")]
    public required DateTime DatePrescribed { get; set; }
}
