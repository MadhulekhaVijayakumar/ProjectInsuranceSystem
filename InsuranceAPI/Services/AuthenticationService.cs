using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using System.Security.Cryptography;
namespace InsuranceAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<string, User> _userRepository;
        private IRepository<int, Client> _clientRepository;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IRepository<string, User> userRpository,
                                     IRepository<int, Client> clientRepository, ITokenService tokenService)
        {
            _userRepository = userRpository;
            _clientRepository = clientRepository;
            _tokenService = tokenService;
        }
        public async Task<LoginResponse> Login(UserLoginRequest loginRequest)
        {
            var user = await _userRepository.GetById(loginRequest.Username);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            HMACSHA512 hmac = new HMACSHA512(user.HashKey);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginRequest.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.Password[i])
                    throw new UnauthorizedAccessException("Invalid password");
            }

            string name = "";
            int id = 0;

            if (user.Role == "Client")
            {
                var client = (await _clientRepository.GetAll()).FirstOrDefault(c => c.Email == loginRequest.Username);
                if (client == null)
                    throw new UnauthorizedAccessException("Client not found");
                name = client.Name;
                id = client.Id;
            }
            else if (user.Role == "Admin")
            {
                // Similar logic if you have AdminRepository
                var admin = user.Admin; // or query from admin repo
                if (admin == null)
                    throw new UnauthorizedAccessException("Admin not found");
                name = admin.Name;
                id = admin.Id;
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid role");
            }

            var token = await _tokenService.GenerateToken(id, name, user.Role);
            return new LoginResponse { Id = id, Name = name, Role = user.Role, Token = token };
        }

    }
}

