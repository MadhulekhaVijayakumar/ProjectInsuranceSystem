using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace InsuranceAPI.Services
{
    public class ClientService : IClientService
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<int, Client> _clientRepository;

        public ClientService(IRepository<string, User> userRepository, IRepository<int, Client> clientRepository)
        {
            _userRepository = userRepository;
            _clientRepository = clientRepository;
        }

        public async Task<CreateClientResponse> CreateClient(CreateClientRequest request)
        {
            HMACSHA512 hmac = new HMACSHA512();
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

            var user = MapClientToUser(request, passwordHash, hmac.Key);
            var userResult = await _userRepository.Add(user);
            if (userResult == null)
                throw new Exception("Failed to create user");

            var client = MapClient(request);
            var clientResult = await _clientRepository.Add(client);
            if (clientResult == null)
                throw new Exception("Failed to create client");

            return new CreateClientResponse { Id = clientResult.Id };
        }

        private Client MapClient(CreateClientRequest request)
        {
            Client client = new Client
            {
                Name = request.Name,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            };
            return client;
        }

        private User MapClientToUser(CreateClientRequest request, byte[] passwordHash, byte[] key)
        {
            User user = new User
            {
                Username = request.Email,
                Password = passwordHash,
                HashKey = key
            };
            return user;
        }
    }
}
