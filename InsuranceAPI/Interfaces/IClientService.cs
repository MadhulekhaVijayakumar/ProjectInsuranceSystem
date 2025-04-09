using InsuranceAPI.Models.DTOs;
using System.Threading.Tasks;

namespace InsuranceAPI.Interfaces
{
    public interface IClientService
    {
        Task<CreateClientResponse> CreateClient(CreateClientRequest request);
        Task<ClientProfileResponse> GetClientProfile(int clientId);
        Task<ClientProfileResponse> UpdateClientProfile(int clientId, UpdateClientRequest request);
    }
}
