namespace ScooterRental.Models
{
    public class RentalRecord
    {
        public RentalRecord(string scooterId, decimal totalCost, DateTime rentStartTime, DateTime rentEndTime)
        {
            ScooterId = scooterId;
            TotalCost = totalCost;
            RentStartTime = rentStartTime;
            RentEndTime = rentEndTime;
        }

        public string ScooterId { get; }
        public decimal TotalCost { get; }
        public DateTime RentStartTime { get; }
        public DateTime RentEndTime { get; }
    }
}
