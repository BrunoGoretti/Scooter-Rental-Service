namespace ScooterRental.Tests
{
    public class ScooterServiceTest
    {
        private ScooterService CreateService()
        {
            return new ScooterService();
        }

        [Fact]
        public void AddScooter_ScooterAddedSuccessfully()
        {
            // Arrange
            var service = CreateService();
            var scooterId = "1";
            var pricePerMinute = 0.20m;

            // Act
            service.AddScooter(scooterId, pricePerMinute);
            var scooter = service.GetScooterById(scooterId);

            // Assert
            Assert.NotNull(scooter);
            Assert.Equal(scooterId, scooter.Id);
            Assert.Equal(pricePerMinute, scooter.PricePerMinute);
        }

        [Fact]
        public void AddScooter_ScooterWithDuplicateId_ThrowsArgumentException()
        {
            // Arrange
            var service = CreateService();
            var scooterId = "1";
            var pricePerMinute = 0.20m;
            service.AddScooter(scooterId, pricePerMinute);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.AddScooter(scooterId, pricePerMinute));
        }

        [Fact]
        public void RemoveScooter_ScooterRemovedSuccessfully()
        {
            // Arrange
            var service = CreateService();
            var scooterId = "1";
            var pricePerMinute = 0.20m;
            service.AddScooter(scooterId, pricePerMinute);

            // Act
            service.RemoveScooter(scooterId);

            // Assert
            Assert.Null(service.GetScooterById(scooterId));
        }

        [Fact]
        public void RemoveScooter_ScooterNotFound_ThrowsArgumentException()
        {
            // Arrange
            var service = CreateService();
            var scooterId = "1";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.RemoveScooter(scooterId));
        }

        [Fact]
        public void RemoveScooter_ScooterCurrentlyRented_ThrowsInvalidOperationException()
        {
            // Arrange
            var service = CreateService();
            var scooterId = "1";
            var pricePerMinute = 0.20m;
            service.AddScooter(scooterId, pricePerMinute);

            // Act
            var scooter = service.GetScooterById(scooterId);
            scooter.IsRented = true;

            // Assert
            Assert.Throws<InvalidOperationException>(() => service.RemoveScooter(scooterId));
        }

        [Fact]
        public void GetScooters_ReturnsAllScooters()
        {
            // Arrange
            var service = CreateService();
            service.AddScooter("1", 0.20m);
            service.AddScooter("2", 0.25m);

            // Act
            var scooters = service.GetScooters();

            // Assert
            Assert.Equal(2, scooters.Count);
        }

        [Fact]
        public void GetScooterById_ScooterExists_ReturnsScooter()
        {
            // Arrange
            var service = CreateService();
            var scooterId = "1";
            var pricePerMinute = 0.20m;
            service.AddScooter(scooterId, pricePerMinute);

            // Act
            var scooter = service.GetScooterById(scooterId);

            // Assert
            Assert.NotNull(scooter);
            Assert.Equal(scooterId, scooter.Id);
        }

        [Fact]
        public void GetScooterById_ScooterNotFound_ReturnsNull()
        {
            // Arrange
            var service = CreateService();

            // Act
            var scooter = service.GetScooterById("1");

            // Assert
            Assert.Null(scooter);
        }
    }
}