using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using InsuranceAPI.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests
{
    [TestFixture]
    public class InsuranceClaimServiceTest
    {
        private Mock<IRepository<int, InsuranceClaim>> _claimRepoMock;
        private Mock<IRepository<string, Insurance>> _insuranceRepoMock;
        private Mock<IDocumentService> _documentServiceMock;
        private InsuranceClaimService _claimService;

        [SetUp]
        public void SetUp()
        {
            // Mocking the repositories and services
            _claimRepoMock = new Mock<IRepository<int, InsuranceClaim>>();
            _insuranceRepoMock = new Mock<IRepository<string, Insurance>>();
            _documentServiceMock = new Mock<IDocumentService>();

            // Instantiate the service with mocked dependencies
            _claimService = new InsuranceClaimService(
                _claimRepoMock.Object,
                _insuranceRepoMock.Object,
                _documentServiceMock.Object
            );
        }

        [Test]
        public async Task SubmitClaimWithDocumentsAsync_ShouldReturnCreatedClaimResponse()
        {
            // Arrange
            var createClaimRequest = new CreateClaimRequest
            {
                InsurancePolicyNumber = "AQ501001",
                IncidentDate = DateTime.Now.AddDays(-1),
                Description = "Accident on the road"
            };

            var clientId = "12345"; // Mocked client ID
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, clientId)
            }));

            var insurance = new Insurance
            {
                InsurancePolicyNumber = "AQ501001",
                ClientId = 12345
            };

            var newClaim = new InsuranceClaim
            {
                ClaimId = 1,
                InsurancePolicyNumber = "AQ501001",
                IncidentDate = DateTime.Now.AddDays(-1),
                Description = "Accident on the road",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var createClaimResponse = new CreateClaimResponse
            {
                ClaimId = 1,
                Status = "Pending",
                Message = "Claim submitted successfully"
            };

            // Setting up mocks
            _insuranceRepoMock.Setup(x => x.GetById(createClaimRequest.InsurancePolicyNumber)).ReturnsAsync(insurance);
            _claimRepoMock.Setup(x => x.Add(It.IsAny<InsuranceClaim>())).ReturnsAsync(newClaim);

            // Act
            var result = await _claimService.SubmitClaimWithDocumentsAsync(createClaimRequest, claimsPrincipal);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(createClaimResponse.ClaimId, result.ClaimId);
            Assert.AreEqual(createClaimResponse.Status, result.Status);
            Assert.AreEqual(createClaimResponse.Message, result.Message);
        }

        [Test]
        public async Task GetClaimsByClientAsync_ShouldReturnClientClaims()
        {
            // Arrange
            var clientId = "12345"; // Mocked client ID
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, clientId)
            }));

            var claims = new List<InsuranceClaim>
            {
                new InsuranceClaim
                {
                    ClaimId = 1,
                    InsurancePolicyNumber = "AQ501001",
                    IncidentDate = DateTime.Now.AddDays(-1),
                    Description = "Accident on the road",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    Insurance = new Insurance { ClientId = 12345 }
                },
                new InsuranceClaim
                {
                    ClaimId = 2,
                    InsurancePolicyNumber = "AQ501002",
                    IncidentDate = DateTime.Now.AddDays(-2),
                    Description = "Hit-and-run accident",
                    Status = "Approved",
                    CreatedAt = DateTime.UtcNow,
                    Insurance = new Insurance { ClientId = 12345 }
                }
            };

            _claimRepoMock.Setup(x => x.GetAll()).ReturnsAsync(claims);

            // Act
            var result = await _claimService.GetClaimsByClientAsync(claimsPrincipal);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task UpdateClaimStatusAsync_ShouldUpdateStatus()
        {
            // Arrange
            var claimId = 1;
            var newStatus = "Approved";
            var claim = new InsuranceClaim
            {
                ClaimId = claimId,
                InsurancePolicyNumber = "AQ501001",
                IncidentDate = DateTime.Now.AddDays(-1),
                Description = "Accident on the road",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _claimRepoMock.Setup(x => x.GetById(claimId)).ReturnsAsync(claim);
            _claimRepoMock.Setup(x => x.Update(claimId, It.IsAny<InsuranceClaim>())).ReturnsAsync(claim);

            // Act
            var result = await _claimService.UpdateClaimStatusAsync(claimId, newStatus);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(newStatus, result.Status);
        }

        [Test]
        public async Task GetAllClaimsForAdminAsync_ShouldReturnClaimsForAdmin()
        {
            // Arrange
            var claims = new List<InsuranceClaim>
            {
                new InsuranceClaim
                {
                    ClaimId = 1,
                    InsurancePolicyNumber = "AQ501001",
                    IncidentDate = DateTime.Now.AddDays(-1),
                    Description = "Accident on the road",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                },
                new InsuranceClaim
                {
                    ClaimId = 2,
                    InsurancePolicyNumber = "AQ501002",
                    IncidentDate = DateTime.Now.AddDays(-2),
                    Description = "Hit-and-run accident",
                    Status = "Approved",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _claimRepoMock.Setup(x => x.GetAll()).ReturnsAsync(claims);

            // Act
            var result = await _claimService.GetAllClaimsForAdminAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }
    }
}
