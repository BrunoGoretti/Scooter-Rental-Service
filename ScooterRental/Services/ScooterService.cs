using ScooterRental.Models;
using ScooterRental.Services.Interfaces;

namespace ScooterRental.Services
{
    public class ScooterService : IScooterService
    {
        private List<Scooter> _scooters = new List<Scooter>();

        public void AddScooter(string id, decimal pricePerMinute)
        {
            if (_scooters.Any(s => s.Id == id))
                throw new ArgumentException("Scooter with this ID already exists.");

            _scooters.Add(new Scooter(id, pricePerMinute));
        }

        public void RemoveScooter(string id)
        {
            var scooter = GetScooterById(id);
            if (scooter == null)
                throw new ArgumentException("Scooter not found.");

            if (scooter.IsRented)
                throw new InvalidOperationException("Cannot remove a scooter that is currently rented.");

            _scooters.Remove(scooter);
        }

        public IList<Scooter> GetScooters()
        {
            return _scooters;
        }

        public Scooter GetScooterById(string scooterId)
        {
            return _scooters.SingleOrDefault(s => s.Id == scooterId);
        }
    }
}
