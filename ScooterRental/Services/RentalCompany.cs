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

            _ongoingRentals[id] = rentalRecord;
        }

        public decimal EndRent(string id)
        {
            if (!_ongoingRentals.ContainsKey(id))
                throw new ArgumentException("Rental not found.");

            var rentalRecord = _ongoingRentals[id];
            var scooter = _scooterService.GetScooterById(id);
            if (scooter == null)
                throw new ArgumentException("Scooter not found.");

            scooter.IsRented = false;
            rentalRecord.RentEndTime = DateTime.Now;

            var totalCost = CalculateRentalCost(rentalRecord, scooter.PricePerMinute);
            rentalRecord.TotalCost = totalCost;

            _rentalRecords.Add(rentalRecord);
            _ongoingRentals.Remove(id);

            return totalCost;
        }

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            var totalIncome = _rentalRecords.Where(r => !year.HasValue || r.RentEndTime.Year == year.Value).Sum(r => r.TotalCost);

            if (includeNotCompletedRentals)
            {
                var ongoingIncome = _ongoingRentals.Values
                    .Where(r => !year.HasValue || r.RentStartTime.Year == year.Value)
                    .Sum(r => CalculateRentalCost(r, _scooterService.GetScooterById(r.ScooterId).PricePerMinute, true));

                totalIncome += ongoingIncome;
            }

            return totalIncome;
        }

        private decimal CalculateRentalCost(RentalRecord rentalRecord, decimal pricePerMinute, bool forOngoing = false)
        {
            var rentEndTime = forOngoing ? DateTime.Now : rentalRecord.RentEndTime;
            var totalMinutes = (decimal)(rentEndTime - rentalRecord.RentStartTime).TotalMinutes;
            var days = (rentEndTime - rentalRecord.RentStartTime).Days + 1;

            var dailyCost = Math.Min(totalMinutes * pricePerMinute, 20 * days);
            return dailyCost;
        }

        /* public decimal EndRent(string id)
         {
             if (!_ongoingRentals.ContainsKey(id))
                 throw new ArgumentException("Rental not started for this scooter.");

             var scooter = _scooterService.GetScooterById(id);
             if (scooter == null)
                 throw new ArgumentException("Scooter not found.");

             if (!scooter.IsRented)
                 throw new InvalidOperationException("Scooter is not currently rented.");

             var rentalRecord = _ongoingRentals[id];
             var endTime = DateTime.Now;

             // Calculate total cost
             decimal totalCost = CalculateRentalCost(scooter.PricePerMinute, rentalRecord.RentStartTime, endTime);
             var completedRentalRecord = new RentalRecord(rentalRecord.ScooterId, totalCost, rentalRecord.RentStartTime, endTime);

             // Record rental
             _rentalRecords.Add(completedRentalRecord);

             // Clean up
             scooter.IsRented = false;
             _ongoingRentals.Remove(id);

             return totalCost;
         }

         public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
         {
             var now = DateTime.Now;
             var records = _rentalRecords.Where(r => !year.HasValue || r.RentStartTime.Year == year.Value).ToList();

             if (includeNotCompletedRentals)
             {
                 records.AddRange(_ongoingRentals.Values.Select(record =>
                     new RentalRecord(
                         record.ScooterId,
                         CalculateRentalCost(_scooterService.GetScooterById(record.ScooterId).PricePerMinute, record.RentStartTime, now),
                         record.RentStartTime,
                         now
                     )));
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
         }*/
    }
}