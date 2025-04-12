using InsuranceAPI.Context;
using InsuranceAPI.Models;
using InsuranceAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests.Repositories
{
    public class ProposalRepositoryTests
    {
        private InsuranceManagementContext _context;
        private ProposalRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InsuranceManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InsuranceManagementContext(options);

            // Seed data
            var client = new Client
            {
                Id = 1,
                Name = "John",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                PhoneNumber = "1234567890",
                Email = "john@example.com",
                AadhaarNumber = "123456789012",
                PANNumber = "ABCDE1234F",
                Address = "Chennai"
            };

            var vehicle = new Vehicle
            {
                VehicleId = 1,
                ClientId = 1,
                VehicleType = "Car",
                VehicleNumber = "TN01AB1234",
                ChassisNumber = "CH123",
                EngineNumber = "EN123",
                MakerName = "Maruti",
                ModelName = "Swift",
                VehicleColor = "Red",
                FuelType = "Petrol",
                RegistrationDate = DateTime.Today.AddYears(-1),
                SeatCapacity = 5
            };

            var proposal = new Proposal
            {
                ProposalId = 1,
                Client = client,
                Vehicle = vehicle,
                //ProposalStatus = "Pending"
            };

            _context.Clients.Add(client);
            _context.Vehicles.Add(vehicle);
            _context.Proposals.Add(proposal);
            _context.SaveChanges();

            _repository = new ProposalRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        //[Test]
        //public async Task GetById_ShouldReturnProposal_WhenExists()
        //{
        //    var result = await _repository.GetById(1);

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(1, result.ProposalId);
        //    Assert.AreEqual("Pending", result.ProposalStatus);
        //}

        [Test]
        public void GetById_ShouldThrowException_WhenNotFound()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () => await _repository.GetById(99));
            Assert.That(ex.Message, Is.EqualTo("Proposal with ID 99 not found"));
        }

        [Test]
        public async Task GetAll_ShouldReturnAllProposals()
        {
            var result = await _repository.GetAll();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }
    }
}