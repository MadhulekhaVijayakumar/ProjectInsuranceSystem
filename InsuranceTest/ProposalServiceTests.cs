using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using InsuranceAPI.Services;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace InsuranceAPI.Tests
{
    public class ProposalServiceTests
    {
        private Mock<IRepository<int, Proposal>> _mockProposalRepo;
        private Mock<IRepository<int, Vehicle>> _mockVehicleRepo;
        private Mock<IRepository<int, InsuranceDetails>> _mockInsuranceRepo;
        private Mock<IPremiumCalculatorService> _mockPremiumService;
        private Mock<IDocumentService> _mockDocumentService;
        private Mock<ILogger<ProposalService>> _mockLogger;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        private ProposalService _service;

        [SetUp]
        public void Setup()
        {
            _mockProposalRepo = new Mock<IRepository<int, Proposal>>();
            _mockVehicleRepo = new Mock<IRepository<int, Vehicle>>();
            _mockInsuranceRepo = new Mock<IRepository<int, InsuranceDetails>>();
            _mockPremiumService = new Mock<IPremiumCalculatorService>();
            _mockDocumentService = new Mock<IDocumentService>();
            _mockLogger = new Mock<ILogger<ProposalService>>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            _mockHttpContextAccessor.Setup(a => a.HttpContext!.User).Returns(user);

            _service = new ProposalService(
                _mockProposalRepo.Object,
                _mockVehicleRepo.Object,
                _mockInsuranceRepo.Object,
                _mockPremiumService.Object,
                _mockDocumentService.Object,
                _mockLogger.Object,
                _mockHttpContextAccessor.Object);
        }

        [Test]
        public async Task SubmitProposalWithDetails_ShouldLogInformationMessages()
        {
            // Arrange
            var request = new CreateProposalRequest
            {
                Vehicle = new CreateVehicleRequest
                {
                    VehicleType = "Car",
                    VehicleNumber = "KA01AB1234",
                    ChassisNumber = "CHASSIS123",
                    EngineNumber = "ENGINE123",
                    MakerName = "Toyota",
                    ModelName = "Corolla",
                    VehicleColor = "White",
                    FuelType = "Petrol",
                    RegistrationDate = DateTime.Today.AddYears(-1),
                    SeatCapacity = 5
                },
                Proposal = new CreateProposalData
                {
                    InsuranceType = "Comprehensive",
                    InsuranceValidUpto = DateTime.Today.AddYears(1),
                    FitnessValidUpto = DateTime.Today.AddYears(1)
                },
                InsuranceDetails = new CreateInsuranceDetailRequest
                {
                    InsuranceStartDate = DateTime.Today,
                    InsuranceSum = 500000,
                    DamageInsurance = "Full",
                    LiabilityOption = "ThirdParty",
                    Plan = "Gold"
                }
            };

            _mockVehicleRepo.Setup(r => r.Add(It.IsAny<Vehicle>())).ReturnsAsync(new Vehicle { VehicleId = 10 });
            _mockProposalRepo.Setup(r => r.Add(It.IsAny<Proposal>())).ReturnsAsync(new Proposal { ProposalId = 100 });
            _mockPremiumService.Setup(p => p.CalculatePremium(It.IsAny<InsuranceDetails>(), It.IsAny<Vehicle>())).Returns(6500);

            // Act
            var result = await _service.SubmitProposalWithDetails(request);

            // Assert logging
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting proposal submission process.")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Vehicle created with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Proposal created with ID")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public void SubmitProposalWithDetails_ShouldLogError_WhenExceptionOccurs()
        {
            // Arrange
            var request = new CreateProposalRequest
            {
                Vehicle = new CreateVehicleRequest
                {
                    VehicleType = "Car",
                    VehicleNumber = "KA01AB1234",
                    ChassisNumber = "CHASSIS123",
                    EngineNumber = "ENGINE123",
                    MakerName = "Toyota",
                    ModelName = "Corolla",
                    VehicleColor = "White",
                    FuelType = "Petrol",
                    RegistrationDate = DateTime.Today.AddYears(-1),
                    SeatCapacity = 5
                },
                Proposal = new CreateProposalData
                {
                    InsuranceType = "Comprehensive",
                    InsuranceValidUpto = DateTime.Today.AddYears(1),
                    FitnessValidUpto = DateTime.Today.AddYears(1)
                },
                InsuranceDetails = new CreateInsuranceDetailRequest
                {
                    InsuranceStartDate = DateTime.Today,
                    InsuranceSum = 500000,
                    DamageInsurance = "Full",
                    LiabilityOption = "ThirdParty",
                    Plan = "Gold"
                }
            };

            // Simulate exception in vehicle creation
            _mockVehicleRepo.Setup(r => r.Add(It.IsAny<Vehicle>()))
                            .ThrowsAsync(new Exception("Database insert failed"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _service.SubmitProposalWithDetails(request));

            Assert.That(ex?.Message, Is.EqualTo("Database insert failed"));

            // Assert error logging
            _mockLogger.Verify(
            x => x.Log<It.IsAnyType>(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error during proposal submission")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        }

    }
}
