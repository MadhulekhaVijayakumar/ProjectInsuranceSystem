using AutoMapper;
using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using InsuranceAPI.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace InsuranceAPITests.Services
{
    public class ClientServiceTests
    {
        private IMapper _mapper;
        private ClientService _clientService;
        private IRepository<string, User> _userRepository;
        private IRepository<int, Client> _clientRepository;

        [SetUp]
        public void Setup()
        {
            // InMemory DbContext
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(databaseName: "InsuranceTestDB")
                .Options;

            var context = new InsuranceManagementContext(options);

            _userRepository = new UserRepository(context);
            _clientRepository = new ClientRepository(context);

            // AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Client, ClientProfileResponse>();
            });
            _mapper = config.CreateMapper();

            _clientService = new ClientService(_userRepository, _clientRepository, _mapper);
        }

        [Test]
        public async Task CreateClient_ShouldReturnClientResponse()
        {
            // Arrange
            var request = new CreateClientRequest
            {
                Name = "Karthi",
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = "Male",
                PhoneNumber = "9876543210",
                Email = "karthi@gmail.com",
                AadhaarNumber = "123412341234",
                PANNumber = "ABCDE1234F",
                Address = "Chennimalai",
                Password = "Password123"
            };

            // Act
            var result = await _clientService.CreateClient(request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetClientProfile_ShouldReturnProfile()
        {
            // Arrange
            var request = new CreateClientRequest
            {
                Name = "Karthi",
                DateOfBirth = new DateTime(2000, 1, 1),
                Gender = "Male",
                PhoneNumber = "9876543210",
                Email = "karthi2@gmail.com",
                AadhaarNumber = "567856785678",
                PANNumber = "WXYZ1234L",
                Address = "Erode",
                Password = "Secret123"
            };

            var createResponse = await _clientService.CreateClient(request);

            // Act
            var profile = await _clientService.GetClientProfile(createResponse.Id);

            // Assert
            Assert.That(profile.Name, Is.EqualTo("Karthi"));
            Assert.That(profile.Email, Is.EqualTo("karthi2@gmail.com"));
        }

        [Test]
        public async Task UpdateClientProfile_ShouldUpdateSuccessfully()
        {
            // Arrange
            var request = new CreateClientRequest
            {
                Name = "Karthik",
                DateOfBirth = new DateTime(1999, 5, 5),
                Gender = "Male",
                PhoneNumber = "9000000000",
                Email = "karthik@gmail.com",
                AadhaarNumber = "999988887777",
                PANNumber = "PQRS1234T",
                Address = "Salem",
                Password = "NewPass123"
            };

            var created = await _clientService.CreateClient(request);

            var updateRequest = new UpdateClientRequest
            {
                NameUpdate = new NameUpdate { NewName = "Karthikeyan" },
                PhoneUpdate = new PhoneUpdate { NewPhoneNumber = "9123456789" }
            };

            // Act
            var updatedProfile = await _clientService.UpdateClientProfile(created.Id, updateRequest);

            // Assert
            Assert.That(updatedProfile.Name, Is.EqualTo("Karthikeyan"));
            Assert.That(updatedProfile.PhoneNumber, Is.EqualTo("9123456789"));
        }
    }
}