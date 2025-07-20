using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VituraHealthWebApi.Models;
using VituraHealthWebApi.Services.Interfaces;

namespace VituraHealthWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly ILogger<PrescriptionsController> _logger;
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionsController(ILogger<PrescriptionsController> logger, IPrescriptionService prescriptionService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _prescriptionService = prescriptionService ?? throw new ArgumentNullException(nameof(prescriptionService));
        }

        [HttpGet]
        public async Task<ActionResult<IList<Prescription>>> GetPrescriptions()
        {
            var prescriptions = await _prescriptionService.GetAllPrescriptionsAsync();
            if (prescriptions == null || !prescriptions.Any())
            {
                _logger.LogWarning("No prescriptions found.");
                return NotFound();
            }
            return Ok(prescriptions);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionRequest createPrescriptionRequest)
        {
            if (createPrescriptionRequest == null)
            {
                _logger.LogError("CreatePrescriptionRequest is null.");
                return BadRequest("Invalid prescription data.");
            }

            var prescribedDate = DateTime.UtcNow; // Use current date and time for the prescription
            await _prescriptionService.CreatePrescriptionAsync(createPrescriptionRequest, prescribedDate);
            return Created();
        }
    }
}
