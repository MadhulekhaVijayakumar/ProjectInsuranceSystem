using InsuranceAPI.Services;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InsuranceAPI.Tests
{
    public class TokenServiceTests
    {
        private TokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Keys:JwtToken", "this_is_a_very_secure_key_1234567890"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new TokenService(configuration);
        }

        [Test]
        public async Task GenerateToken_ValidData_ReturnsToken()
        {
            // Arrange
            int id = 1;
            string name = "Karthi";
            string role = "Client";

            // Act
            var token = await _tokenService.GenerateToken(id, name, role);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsNotEmpty(token);
            TestContext.WriteLine($"Generated Token: {token}");
        }
    }
}