using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using System.Security.Claims;

namespace InsuranceAPI.Interfaces
{
    public interface IInsuranceClaimService
    {
        Task<CreateClaimResponse> SubmitClaimWithDocumentsAsync(CreateClaimRequest request, ClaimsPrincipal user);
        Task<IEnumerable<InsuranceClaim>> GetClaimsByClientAsync(ClaimsPrincipal user);
        Task<IEnumerable<InsuranceClaimDto>> GetAllClaimsForAdminAsync();
        Task<InsuranceClaim> UpdateClaimStatusAsync(int claimId, string newStatus);
    }
}
