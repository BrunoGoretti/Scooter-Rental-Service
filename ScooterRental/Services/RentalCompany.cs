using ScooterRental.Models;
using ScooterRental.Services.Interfaces;

namespace ScooterRental.Services
{
    public class RentalCompany : IRentalCompany
    {
        private readonly IScooterService _scooterService;
        private readonly Dictionary<string, DateTime> _rentalStartTimes = new Dictionary<string, DateTime>();
        private readonly List<RentalRecord> _rentalRecords = new List<RentalRecord>();

        public RentalCompany(IScooterService scooterService)
        {
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
            _rentalStartTimes[id] = DateTime.Now;
        }

        public decimal EndRent(string id)
        {
            if (!_rentalStartTimes.ContainsKey(id))
                throw new ArgumentException("Rental not started for this scooter.");

            var scooter = _scooterService.GetScooterById(id);
            if (scooter == null)
                throw new ArgumentException("Scooter not found.");

            if (!scooter.IsRented)
                throw new InvalidOperationException("Scooter is not currently rented.");

            var startTime = _rentalStartTimes[id];
            var endTime = DateTime.Now;

            // Calculate total cost
            decimal totalCost = CalculateRentalCost(scooter.PricePerMinute, startTime, endTime);

            // Record rental
            _rentalRecords.Add(new RentalRecord(id, totalCost, startTime, endTime));

            // Clean up
            scooter.IsRented = false;
            _rentalStartTimes.Remove(id);

            return totalCost;
        }

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            var now = DateTime.Now;
            var records = _rentalRecords.Where(r => !year.HasValue || r.RentStartTime.Year == year.Value).ToList();

            if (includeNotCompletedRentals)
            {
                records.AddRange(_rentalStartTimes.Select(kv => new RentalRecord(
                    kv.Key,
                    CalculateRentalCost(_scooterService.GetScooterById(kv.Key).PricePerMinute, kv.Value, now),
                    kv.Value,
                    now)));
            }

            return records.Sum(r => r.TotalCost);
        }

        private decimal CalculateRentalCost(decimal pricePerMinute, DateTime startTime, DateTime endTime)
        {
            var totalMinutes = (decimal)(endTime - startTime).TotalMinutes;
            var totalDays = Math.Ceiling(totalMinutes / (24 * 60));

            // If total amount per day reaches 20 EUR, reset timer at 0:00 am next day
            var dailyCost = 20m;
            return Math.Min(totalDays * dailyCost, totalMinutes * pricePerMinute);
        }

    }
}
