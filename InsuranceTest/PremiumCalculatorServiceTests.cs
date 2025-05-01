using InsuranceAPI.Models;
using InsuranceAPI.Services;
using NUnit.Framework;

namespace InsuranceAPITests
{
    [TestFixture]
    public class PremiumCalculatorServiceTests
    {
        private PremiumCalculatorService _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = new PremiumCalculatorService();
        }

        [Test]
        public void CalculatePremium_WithTypicalCarDieselSilverPartial_ReturnsExpectedPremium()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                VehicleType = "Car",
                FuelType = "Diesel",
                SeatCapacity = 5
            };

            var details = new InsuranceDetails
            {
                InsuranceSum = 100000,
                Plan = "Silver",
                DamageInsurance = "Partial",
                LiabilityOption = "Own Damage"
            };

            // Act
            var result = _calculator.CalculatePremium(details, vehicle);

            // Base = 3% of 100000 = 3000
            // + Car(1000) + Diesel(1000) + Silver(500) + Partial(750) + Own Damage(1000)
            // Total = 3000 + 4250 = 7250

            // Assert
            Assert.AreEqual(7250, result);
        }

        [Test]
        public void CalculatePremium_MinimumPremium_ReturnsAtLeast2000()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                VehicleType = "Bike",
                FuelType = "Petrol",
                SeatCapacity = 2
            };

            var details = new InsuranceDetails
            {
                InsuranceSum = 100,
                Plan = "Silver",
                DamageInsurance = "None",
                LiabilityOption = "None"
            };

            // Act
            var result = _calculator.CalculatePremium(details, vehicle);

            // Assert
            Assert.GreaterOrEqual(result, 2000);
        }
    }
}