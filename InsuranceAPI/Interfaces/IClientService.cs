using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using System.Threading.Tasks;

namespace InsuranceAPI.Interfaces
{
    public interface IClientService
    {
        Task<CreateClientResponse> CreateClient(CreateClientRequest request);
        Task<ClientProfileResponse> GetClientProfile(int clientId);
        Task<ClientProfileResponse> UpdateClientProfile(int clientId, UpdateClientRequest request);

        Task<IEnumerable<ClientProfileResponse>> GetAllClients();
        Task<IEnumerable<ClientProfileResponse>> SearchClients(string keyword);
        Task<bool> ChangeClientPassword(string email, string oldPassword, string newPassword);


    }
}
