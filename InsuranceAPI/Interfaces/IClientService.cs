using InsuranceAPI.Models.DTOs;

namespace InsuranceAPI.Interfaces
{
    public interface IClientService
    {
        Task<CreateClientResponse> CreateClient(CreateClientRequest request);
    }
}
