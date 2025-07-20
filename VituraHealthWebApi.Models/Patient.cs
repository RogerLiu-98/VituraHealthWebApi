using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VituraHealthWebApi.Models;

public class Patient
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fullName")]
    public required string FullName { get; set; }

    [JsonPropertyName("dateOfBirth")]
    public DateTime DateOfBirth { get; set; }
}
