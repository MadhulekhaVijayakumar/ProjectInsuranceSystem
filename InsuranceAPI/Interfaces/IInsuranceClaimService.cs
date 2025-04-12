using InsuranceAPI.Models.DTOs;

namespace InsuranceAPI.Interfaces
{
    public interface IInsuranceClaimService
    {
        Task<CreateClaimResponse> SubmitClaimAsync(CreateClaimRequest request);
        Task<IEnumerable<ClaimSummaryDto>> GetClaimsForClient(int clientId);
        Task<IEnumerable<ClaimSummaryDto>> GetAllClaims(); // For admin
        Task<bool> UpdateClaimStatus(int claimId, string status);


    }
}
