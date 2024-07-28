namespace ScooterRental.Tests
{
    public class RentalCompanyTests
    {
        private readonly RentalCompany _rentalCompany;
        private readonly Mock<IScooterService> _scooterServiceMock;

        public RentalCompanyTests()
        {
            _scooterServiceMock = new Mock<IScooterService>();
            _rentalCompany = new RentalCompany(_scooterServiceMock.Object);
        }

        [Fact]
        public void StartRent_ScooterNotFound_ThrowsArgumentException()
        {
            // Arrange
            string scooterId = "invalid-id";
            _scooterServiceMock.Setup(s => s.GetScooterById(scooterId)).Returns((Scooter)null);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _rentalCompany.StartRent(scooterId));
            Assert.Equal("Scooter not found.", ex.Message);
        }

        [Fact]
        public void StartRent_ScooterAlreadyRented_ThrowsInvalidOperationException()
        {
            // Arrange
            string scooterId = "valid-id";
            var scooter = new Scooter(scooterId, 0.2m) { IsRented = true };
            _scooterServiceMock.Setup(s => s.GetScooterById(scooterId)).Returns(scooter);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _rentalCompany.StartRent(scooterId));
            Assert.Equal("Scooter is already rented.", ex.Message);
        }

        [Fact]
        public void StartRent_ValidScooter_StartsRental()
        {
            // Arrange
            string scooterId = "valid-id";
            var scooter = new Scooter(scooterId, 0.2m) { IsRented = false };
            _scooterServiceMock.Setup(s => s.GetScooterById(scooterId)).Returns(scooter);

            // Act
            _rentalCompany.StartRent(scooterId);

            // Assert
            _scooterServiceMock.Verify(s => s.GetScooterById(scooterId), Times.Once);
            Assert.True(scooter.IsRented);
        }

        [Fact]
        public void EndRent_ScooterNotFound_ThrowsArgumentException()
        {
            // Arrange
            string scooterId = "invalid-id";
            _scooterServiceMock.Setup(s => s.GetScooterById(scooterId)).Returns((Scooter)null);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _rentalCompany.EndRent(scooterId));
            Assert.Equal("Scooter not found.", ex.Message);
        }

        [Fact]
        public void EndRent_ScooterNotRented_ThrowsInvalidOperationException()
        {
            // Arrange
            string scooterId = "valid-id";
            var scooter = new Scooter(scooterId, 0.2m) { IsRented = false };
            _scooterServiceMock.Setup(s => s.GetScooterById(scooterId)).Returns(scooter);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _rentalCompany.EndRent(scooterId));
            Assert.Equal("Scooter is not currently rented.", ex.Message);
        }

        [Fact]
        public void EndRent_ValidScooter_EndsRental()
        {
            // Arrange
            string scooterId = "valid-id";
            var scooter = new Scooter(scooterId, 0.2m) { IsRented = false };
            var rentalRecord = new RentalRecord(scooterId, DateTime.Now.AddMinutes(-30));

            _scooterServiceMock.Setup(s => s.GetScooterById(scooterId)).Returns(scooter);

            // Start the rental to ensure the scooter is rented before ending it
            _rentalCompany.StartRent(scooterId);

            // Simulate adding rental record to ongoing rentals
            _rentalCompany.GetType().GetField("_ongoingRentals", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_rentalCompany, new Dictionary<string, RentalRecord> { { scooterId, rentalRecord } });

            // Act
            var totalCost = _rentalCompany.EndRent(scooterId);

            // Assert
            _scooterServiceMock.Verify(s => s.GetScooterById(scooterId), Times.Exactly(2));
            Assert.False(scooter.IsRented);
            Assert.InRange(totalCost, 5.9999m, 6.0001m); // Using a range to account for floating-point precision
        }

        [Fact]
        public void CalculateIncome_CompletedRentals_IncomeCalculatedCorrectly()
        {
            // Arrange
            string scooterId1 = "valid-id-1";
            string scooterId2 = "valid-id-2";
            var rentalRecord1 = new RentalRecord(scooterId1, new DateTime(2024, 7, 1, 10, 0, 0)) { RentEndTime = new DateTime(2024, 7, 1, 10, 30, 0), TotalCost = 6 };
            var rentalRecord2 = new RentalRecord(scooterId2, new DateTime(2023, 6, 30, 10, 0, 0)) { RentEndTime = new DateTime(2023, 6, 30, 11, 0, 0), TotalCost = 12 };
            var rentalRecords = new List<RentalRecord> { rentalRecord1, rentalRecord2 };
            _rentalCompany.GetType().GetField("_rentalRecords", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_rentalCompany, rentalRecords);

            // Act
            var income = _rentalCompany.CalculateIncome(2024, false);

            // Assert
            Assert.Equal(6m, income);
        }

        [Fact]
        public void CalculateIncome_IncludingOngoingRentals_IncomeCalculatedCorrectly()
        {
            // Arrange
            string scooterId1 = "valid-id-1";
            string scooterId2 = "valid-id-2";
            var rentalRecord1 = new RentalRecord(scooterId1, new DateTime(2024, 7, 1, 10, 0, 0)) { RentEndTime = new DateTime(2024, 7, 1, 10, 30, 0), TotalCost = 6 };
            var rentalRecords = new List<RentalRecord> { rentalRecord1 };
            _rentalCompany.GetType().GetField("_rentalRecords", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_rentalCompany, rentalRecords);

            var ongoingRentalRecord = new RentalRecord(scooterId2, DateTime.Now.AddMinutes(-60));
            var ongoingRentals = new Dictionary<string, RentalRecord> { { scooterId2, ongoingRentalRecord } };
            _rentalCompany.GetType().GetField("_ongoingRentals", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_rentalCompany, ongoingRentals);

            var scooter = new Scooter(scooterId2, 0.2m) { IsRented = true };
            _scooterServiceMock.Setup(s => s.GetScooterById(scooterId2)).Returns(scooter);

            // Act
            var income = _rentalCompany.CalculateIncome(2024, true);

            // Assert
            Assert.True(income > 6m); // Should include the ongoing rental cost as well
        }
    }
}