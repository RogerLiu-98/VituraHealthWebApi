using Microsoft.AspNetCore.Mvc;
using VituraHealthWebApi.Models;
using VituraHealthWebApi.Services.Interfaces;

namespace VituraHealthWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
    }

    [HttpGet]
    public async Task<ActionResult<IList<Patient>>> GetAllPatients()
    {
        var patients = await _patientService.GetAllPatientsAsync();
        if (patients == null || !patients.Any())
        {
            _logger.LogInformation("No patients found.");
            return NotFound("No patients found.");
        }
        return Ok(patients);
    }
}
