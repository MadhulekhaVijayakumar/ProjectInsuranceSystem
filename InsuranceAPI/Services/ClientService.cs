﻿using AutoMapper;
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
        private readonly IMapper _mapper;

        public ClientService(IRepository<string, User> userRepository, IRepository<int, Client> clientRepository,IMapper mapper)
        {
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _mapper = mapper;
        }
        public async Task<CreateClientResponse> CreateClient(CreateClientRequest request)
        {
            HMACSHA512 hmac = new HMACSHA512();
            byte[] passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password));
            var User = MapClientToUser(request, passwordHash, hmac.Key);
            var userResult = await _userRepository.Add(User);
            if (userResult == null)
                throw new Exception("Failed to create user");
            var employee = MapClient(request);
            var employeeResult = await _clientRepository.Add(employee);
            if (employeeResult == null)
                throw new Exception("Failed to create employee");
            return new CreateClientResponse { Id = employeeResult.Id };
        }


        private Client MapClient(CreateClientRequest request)
        {
            return new Client
            {
                Name = request.Name,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                AadhaarNumber = request.AadhaarNumber,
                PANNumber = request.PANNumber,
                Address = request.Address
            };
        }

        private User MapClientToUser(CreateClientRequest request, byte[] passwordHash, byte[] key)
        {
            User user = new User
            {
                Username = request.Email,
                Password = passwordHash,
                HashKey = key,
                Role="Client"
            };
            return user;
        }

        public async Task<ClientProfileResponse> GetClientProfile(int clientId)
        {
            var client = await _clientRepository.GetById(clientId);
            return new ClientProfileResponse
            {
                Id = client.Id,
                Name = client.Name,
                DateOfBirth = client.DateOfBirth,
                Gender = client.Gender,
                PhoneNumber = client.PhoneNumber,
                Email = client.Email,
                AadhaarNumber = client.AadhaarNumber,
                PANNumber = client.PANNumber,
                Address = client.Address
            };
        }

        public async Task<ClientProfileResponse> UpdateClientProfile(int clientId, UpdateClientRequest request)
        {
            var client = await _clientRepository.GetById(clientId);
            if (client == null)
                throw new Exception("Client not found");

            if (request.NameUpdate?.NewName != null)
                client.Name = request.NameUpdate.NewName;

            if (request.DateOfBirthUpdate?.NewDateOfBirth != null)
                client.DateOfBirth = request.DateOfBirthUpdate.NewDateOfBirth.Value;

            if (request.GenderUpdate?.NewGender != null)
                client.Gender = request.GenderUpdate.NewGender;

            if (request.PhoneUpdate?.NewPhoneNumber != null)
                client.PhoneNumber = request.PhoneUpdate.NewPhoneNumber;

            if (request.AadhaarUpdate?.NewAadhaarNumber != null)
                client.AadhaarNumber = request.AadhaarUpdate.NewAadhaarNumber;

            if (request.PANUpdate?.NewPANNumber != null)
                client.PANNumber = request.PANUpdate.NewPANNumber;

            if (request.AddressUpdate?.NewAddress != null)
                client.Address = request.AddressUpdate.NewAddress;

            var updatedClient = await _clientRepository.Update(client.Id, client);
            if (updatedClient == null)
                throw new Exception("Failed to update client");

            return _mapper.Map<ClientProfileResponse>(updatedClient);
        }



    }
}
