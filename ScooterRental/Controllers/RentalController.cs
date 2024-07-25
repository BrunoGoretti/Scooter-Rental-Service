using Microsoft.AspNetCore.Mvc;
using ScooterRental.Services.Interfaces;

namespace ScooterRental.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalController : ControllerBase
    {
        private readonly IRentalCompany _rentalCompany;

        public RentalController(IRentalCompany rentalCompany)
        {
            _rentalCompany = rentalCompany;
        }

        [HttpPost("start")]
        public IActionResult StartRent(string id)
        {
            try
            {
                _rentalCompany.StartRent(id);
                return Ok("Rental started successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("end")]
        public IActionResult EndRent(string id)
        {
            try
            {
                decimal totalCost = _rentalCompany.EndRent(id);
                return Ok($"Rental ended successfully. Total cost: {totalCost:C}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("income")]
        public IActionResult CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            var income = _rentalCompany.CalculateIncome(year, includeNotCompletedRentals);
            return Ok($"Total income: {income:C}");
        }
    }
}