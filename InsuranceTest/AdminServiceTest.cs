//using InsuranceAPI.Interfaces;
//using InsuranceAPI.Models;
//using InsuranceAPI.Models.DTOs;
//using InsuranceAPI.Services;
//using Moq;
//using System.Security.Cryptography;
//using System.Text;
//using Xunit;

//namespace InsuranceAPITests.Services
//{
//    public class AdminServiceTests
//    {
//        private readonly Mock<IRepository<string, User>> _mockUserRepo;
//        private readonly Mock<IRepository<int, Admin>> _mockAdminRepo;
//        private readonly AdminService _adminService;

//        public AdminServiceTests()
//        {
//            _mockUserRepo = new Mock<IRepository<string, User>>();
//            _mockAdminRepo = new Mock<IRepository<int, Admin>>();
//            _adminService = new AdminService(_mockUserRepo.Object, _mockAdminRepo.Object);
//        }

//        [Fact]
//        public async Task CreateAdmin_ShouldReturnSuccessResponse_WhenValidRequestGiven()
//        {
//            // Arrange
//            var request = new CreateAdminRequest
//            {
//                Name = "Admin One",
//                Email = "admin@example.com",
//                Password = "securepassword"
//            };

//            var hmac = new HMACSHA512();
//            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

//            var user = new User
//            {
//                Username = request.Email,
//                Password = passwordHash,
//                HashKey = hmac.Key,
//                Role = "Admin"
//            };

//            var admin = new Admin
//            {
//                Id = 1,
//                Name = request.Name,
//                Email = request.Email
//            };

//            _mockUserRepo.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(user);
//            _mockAdminRepo.Setup(repo => repo.Add(It.IsAny<Admin>())).ReturnsAsync(admin);

//            // Act
//            var result = await _adminService.CreateAdmin(request);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(1, result.Id);
//            Assert.Equal("Admin created successfully", result.Message);
//        }

//        [Fact]
//        public async Task CreateAdmin_ShouldThrowException_WhenUserCreationFails()
//        {
//            // Arrange
//            var request = new CreateAdminRequest
//            {
//                Name = "Test Admin",
//                Email = "test@admin.com",
//                Password = "test123"
//            };

//            _mockUserRepo.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync((User?)null);

//            // Act & Assert
//            await Assert.ThrowsAsync<Exception>(() => _adminService.CreateAdmin(request));
//        }

//        [Fact]
//        public async Task CreateAdmin_ShouldThrowException_WhenAdminCreationFails()
//        {
//            // Arrange
//            var request = new CreateAdminRequest
//            {
//                Name = "Test Admin",
//                Email = "test@admin.com",
//                Password = "test123"
//            };

//            var hmac = new HMACSHA512();
//            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
//            var user = new User
//            {
//                Username = request.Email,
//                Password = passwordHash,
//                HashKey = hmac.Key,
//                Role = "Admin"
//            };

//            _mockUserRepo.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(user);
//            _mockAdminRepo.Setup(r => r.Add(It.IsAny<Admin>())).ReturnsAsync((Admin?)null);

//            // Act & Assert
//            await Assert.ThrowsAsync<Exception>(() => _adminService.CreateAdmin(request));
//        }
//    }
//}