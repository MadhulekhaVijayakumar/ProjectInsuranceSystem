using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;

namespace InsuranceAPI.Interfaces
{
    public interface IInsuranceService
    {
        Task<InsuranceResponse> GenerateInsuranceAsync(int proposalId);
        Task<InsuranceResponse> GetInsuranceByProposalIdAsync(int proposalId);
        Task<IEnumerable<ClientPolicyStatusDto>> GetClientPolicyStatusAsync(int clientId);
        Task<InsuranceQuoteResponse> GenerateQuote(int proposalId);
        Task<Insurance> GetInsuranceWithDetailsAsync(string policyNumber);
    }

}
