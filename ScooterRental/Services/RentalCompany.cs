using ScooterRental.Models;
using ScooterRental.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScooterRental.Services
{
    public class RentalCompany : IRentalCompany
    {
        private readonly IScooterService _scooterService;
        private readonly List<RentalRecord> _rentalRecords = new List<RentalRecord>();
        private readonly Dictionary<string, RentalRecord> _ongoingRentals = new Dictionary<string, RentalRecord>();

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
            var rentStartTime = DateTime.Now;
            var rentalRecord = new RentalRecord(id, rentStartTime);

            _ongoingRentals.Add(id, rentalRecord);
        }

        public decimal EndRent(string id)
        {
            var scooter = _scooterService.GetScooterById(id);

            if (scooter == null)
                throw new ArgumentException("Scooter not found.");

            if (!_ongoingRentals.ContainsKey(id))
                throw new InvalidOperationException("Scooter is not currently rented.");
            Console.WriteLine($"Scooter {id} is currently rented. Proceeding to end rent...");

            var rentalRecord = _ongoingRentals[id];
            var rentEndTime = DateTime.Now;
            var totalMinutes = (rentEndTime - rentalRecord.RentStartTime).TotalMinutes;
            var totalCost = (decimal)totalMinutes * scooter.PricePerMinute;

            // Apply daily cap of 20 EUR
            var rentDays = (int)Math.Ceiling(totalMinutes / (24 * 60));
            totalCost = Math.Min(totalCost, rentDays * 20);

            _ongoingRentals.Remove(id);
            scooter.IsRented = false;

            // Update rental record
            rentalRecord.RentEndTime = rentEndTime;
            rentalRecord.TotalCost = totalCost;

            _rentalRecords.Add(rentalRecord);

            return totalCost;
        }

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            var income = _rentalRecords
                .Where(r => !year.HasValue || r.RentStartTime.Year == year.Value)
                .Sum(r => r.TotalCost);

            if (includeNotCompletedRentals)
            {
                var ongoingIncome = _ongoingRentals.Values
                    .Where(r => !year.HasValue || r.RentStartTime.Year == year.Value || DateTime.Now.Year == year.Value)
                    .Sum(r =>
                    {
                        var scooter = _scooterService.GetScooterById(r.ScooterId);
                        var totalMinutes = (DateTime.Now - r.RentStartTime).TotalMinutes;
                        var totalCost = (decimal)totalMinutes * scooter.PricePerMinute;

                        // Apply daily cap of 20 EUR
                        var rentDays = (int)Math.Ceiling(totalMinutes / (24 * 60));
                        return Math.Min(totalCost, rentDays * 20);
                    });

                income += ongoingIncome;
            }

            return income;
        }
    }
}