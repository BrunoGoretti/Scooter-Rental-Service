namespace ScooterRental.Models
{
    public class RentalRecord
    {
        public RentalRecord(string scooterId, DateTime rentStartTime)
        {
            ScooterId = scooterId;
            RentStartTime = rentStartTime;
        }

        public string ScooterId { get; }
        public decimal TotalCost { get; }
        public DateTime RentStartTime { get; }
        public DateTime RentEndTime { get; }
    }
}
