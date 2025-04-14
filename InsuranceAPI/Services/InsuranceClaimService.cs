using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace InsuranceAPI.Services
{
    public class InsuranceClaimService : IInsuranceClaimService
    {
        private readonly IRepository<int, InsuranceClaim> _claimRepository;
        private readonly IRepository<string, Insurance> _insuranceRepository;
        private readonly IDocumentService _documentService;

        public InsuranceClaimService(
            IRepository<int, InsuranceClaim> claimRepository,
            IRepository<string, Insurance> insuranceRepository,
            IDocumentService documentService)
        {
            _claimRepository = claimRepository;
            _insuranceRepository = insuranceRepository;
            _documentService = documentService;
        }


        public async Task<CreateClaimResponse> SubmitClaimWithDocumentsAsync(CreateClaimRequest request, ClaimsPrincipal user)
        {
            var clientId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId))
                throw new Exception("Invalid client information");

            var insurance = await _insuranceRepository.GetById(request.InsurancePolicyNumber);
            if (insurance == null || insurance.ClientId.ToString() != clientId)
                throw new Exception("Invalid insurance policy for this client");

            var claim = new InsuranceClaim
            {
                InsurancePolicyNumber = request.InsurancePolicyNumber,
                IncidentDate = request.IncidentDate,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            var savedClaim = await _claimRepository.Add(claim);

            if (request.Documents != null)
            {
                await _documentService.SaveClaimDocumentsAsync(savedClaim.ClaimId, request.Documents);
            }


            return new CreateClaimResponse
            {
                ClaimId = savedClaim.ClaimId,
                Status = savedClaim.Status,
                Message = "Claim submitted successfully"
            };
        }

        public async Task<IEnumerable<InsuranceClaim>> GetClaimsByClientAsync(ClaimsPrincipal user)
        {
            var clientId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(clientId))
                throw new Exception("Invalid client information");

            var allClaims = await _claimRepository.GetAll();
            return allClaims.Where(c => c.Insurance != null && c.Insurance.ClientId.ToString() == clientId);
        }

        public async Task<IEnumerable<InsuranceClaimDto>> GetAllClaimsForAdminAsync()
        {
            var claims = await _claimRepository.GetAll();
            return claims.Select(c => new InsuranceClaimDto
            {
                ClaimId = c.ClaimId,
                Status = c.Status,
                Description = c.Description,
                InsurancePolicyNumber = c.InsurancePolicyNumber
            });
        }


        public async Task<InsuranceClaim> UpdateClaimStatusAsync(int claimId, string newStatus)
        {
            var claim = await _claimRepository.GetById(claimId);
            if (claim == null)
                throw new Exception("Claim not found");

            if (newStatus != "Approved" && newStatus != "Rejected")
                throw new Exception("Invalid status value. Must be 'Approved' or 'Rejected'.");

            claim.Status = newStatus;
            return await _claimRepository.Update(claimId, claim);
        }
    }
}
