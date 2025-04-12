using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using InsuranceAPI.Interfaces;
using Microsoft.Extensions.Logging;

namespace InsuranceAPI.Services
{
    public class InsuranceClaimService : IInsuranceClaimService
    {
        private readonly InsuranceClaimRepository _claimRepository;
        private readonly IRepository<string, Insurance> _insuranceRepository;
        private readonly ILogger<InsuranceClaimService> _logger;

        public InsuranceClaimService(
            InsuranceClaimRepository claimRepository,
            IRepository<string, Insurance> insuranceRepository,
            ILogger<InsuranceClaimService> logger)
        {
            _claimRepository = claimRepository;
            _insuranceRepository = insuranceRepository;
            _logger = logger;
        }

        public async Task<CreateClaimResponse> SubmitClaimAsync(CreateClaimRequest request)
        {
            try
            {
                var insurance = await _insuranceRepository.GetById(request.InsurancePolicyNumber);
                if (insurance == null)
                    throw new Exception("Invalid Insurance Policy Number.");

                var claim = new InsuranceClaim
                {
                    InsurancePolicyNumber = request.InsurancePolicyNumber,
                    IncidentDate = request.IncidentDate,
                    Description = request.Description,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                var createdClaim = await _claimRepository.Add(claim);

                _logger.LogInformation("Claim submitted successfully with ClaimId: {ClaimId}", createdClaim.ClaimId);

                return new CreateClaimResponse
                {
                    ClaimId = createdClaim.ClaimId,
                    Status = createdClaim.Status,
                    Message = "Claim submitted successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while submitting claim.");
                throw;
            }
        }

        public async Task<IEnumerable<ClaimSummaryDto>> GetClaimsForClient(int clientId)
        {
            var claims = await _claimRepository.GetClaimsForClient(clientId);

            return claims.Select(c => new ClaimSummaryDto
            {
                ClaimId = c.ClaimId,
                PolicyNumber = c.InsurancePolicyNumber,
                IncidentDate = c.IncidentDate,
                Status = c.Status,
                Description = c.Description
            });
        }

        public async Task<IEnumerable<ClaimSummaryDto>> GetAllClaims()
        {
            var claims = await _claimRepository.GetAllClaimsWithInsurance();

            return claims.Select(c => new ClaimSummaryDto
            {
                ClaimId = c.ClaimId,
                PolicyNumber = c.InsurancePolicyNumber,
                IncidentDate = c.IncidentDate,
                Status = c.Status,
                Description = c.Description
            });
        }

        public async Task<bool> UpdateClaimStatus(int claimId, string status)
        {
            var claim = await _claimRepository.GetById(claimId);
            if (claim == null)
                throw new Exception("Claim not found");

            claim.Status = status;
            await _claimRepository.Update(claimId, claim);

            return true;
        }
    }
}
