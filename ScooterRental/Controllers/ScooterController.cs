using Microsoft.AspNetCore.Mvc;
using ScooterRental.Services.Interfaces;

namespace ScooterRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScooterController : ControllerBase
    {
        private readonly IScooterService _scooterService;

        public ScooterController(IScooterService scooterService)
        {
            _scooterService = scooterService;
        }

        [HttpPost("add")]
        public IActionResult AddScooter(string id, decimal pricePerMinute)
        {
            try
            {
                _scooterService.AddScooter(id, pricePerMinute);
                return Ok("Scooter added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("remove")]
        public IActionResult RemoveScooter(string id)
        {
            try
            {
                _scooterService.RemoveScooter(id);
                return Ok("Scooter removed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("list")]
        public IActionResult GetScooters()
        {
            var scooters = _scooterService.GetScooters();
            return Ok(scooters);
        }

        [HttpGet("get")]
        public IActionResult GetScooterById(string id)
        {
            var scooter = _scooterService.GetScooterById(id);
            if (scooter == null)
                return NotFound("Scooter not found.");
            return Ok(scooter);
        }
    }
}