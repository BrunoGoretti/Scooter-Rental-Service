using ScooterRental.Models;
using ScooterRental.Services.Interfaces;

namespace ScooterRental.Services
{
    public class RentalCompany : IRentalCompany
    {
        private IScooterService _scooterService;
        private List<RentalRecord> rentalRecords = new List<RentalRecord>();

        public RentalCompany(string name, IScooterService scooterService)
        {
            Name = name;
            _scooterService = scooterService;
        }

        public string Name { get; }

        public void StartRent(string id)
        {
            var scooter = _scooterService.GetScooterById(id);
            if (scooter == null)
                throw new ArgumentException("Scooter not found.");
            if (scooter.IsRented)
                throw new InvalidOperationException("Scooter is already rented.");

            scooter.IsRented = true;
            scooter.RentStartTime = DateTime.Now;
        }
    }
}
