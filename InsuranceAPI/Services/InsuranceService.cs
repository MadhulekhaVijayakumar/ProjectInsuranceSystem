using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Misc;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Services
{
    public class InsuranceService : IInsuranceService
    {
        private readonly InsuranceManagementContext _context;
        private readonly ILogger<InsuranceService> _logger;
        private readonly InsurancePolicyNumberGenerator _policyNumberGenerator;

        public InsuranceService(InsuranceManagementContext context, ILogger<InsuranceService> logger, InsurancePolicyNumberGenerator policyNumberGenerator)
        {
            _context = context;
            _logger = logger;
            _policyNumberGenerator = policyNumberGenerator;
        }


        public async Task<InsuranceQuoteResponse> GenerateQuote(int proposalId)
        {
            // Fetch the proposal
            var proposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.ProposalId == proposalId);

            if (proposal == null)
                throw new Exception("Proposal not found");

            if (proposal.Status != "approved")
                throw new Exception("Proposal is not approved");

            // Fetch the related insurance details
            var insuranceDetails = await _context.InsuranceDetails
                .FirstOrDefaultAsync(i => i.ProposalId == proposalId);

            if (insuranceDetails == null)
                throw new Exception("Insurance details not found");

            // Update the status in InsuranceDetails to "quote generated"
            proposal.Status = "quote generated";
            await _context.SaveChangesAsync();

            // Return the quote response
            return new InsuranceQuoteResponse
            {
                ProposalId = proposalId,
                PremiumAmount = insuranceDetails.CalculatedPremium,
                InsuranceSum = insuranceDetails.InsuranceSum,
                InsuranceStartDate = insuranceDetails.InsuranceStartDate,
                Status = proposal.Status
            };
        }


        public async Task<InsuranceResponse> GenerateInsuranceAsync(int proposalId)
        {
            // Fetch related insurance details
            var insuranceDetails = await _context.InsuranceDetails
                .FirstOrDefaultAsync(i => i.ProposalId == proposalId);

            if (insuranceDetails == null)
            {
                _logger.LogError("Insurance details not found for proposal ID {ProposalId}", proposalId);
                throw new Exception("Insurance details not found.");
            }

            // Fetch the proposal
            var proposal = await _context.Proposals.FindAsync(proposalId);
            if (proposal == null)
            {
                _logger.LogError("Proposal not found for ID {ProposalId}", proposalId);
                throw new Exception("Proposal not found.");
            }

            // Generate Insurance Policy Number using stored procedure
            var policyNumber = await _policyNumberGenerator.GeneratePolicyNumber();

            // Create Insurance object
            var insurance = new Insurance
            {
                InsurancePolicyNumber = policyNumber,
                ProposalId = proposalId,
                VehicleId = proposal.VehicleId,
                ClientId = proposal.ClientId,
                PremiumAmount = insuranceDetails.CalculatedPremium,
                InsuranceStartDate = insuranceDetails.InsuranceStartDate,
                InsuranceSum = insuranceDetails.InsuranceSum,
                Status = "active",
                CreatedAt = DateTime.Now
            };

            _context.Insurances.Add(insurance);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Insurance created with Policy Number {PolicyNumber} for Proposal ID {ProposalId}", policyNumber, proposalId);

            // Return response
            return new InsuranceResponse
            {
                ProposalId = proposalId,
                VehicleId = proposal.VehicleId,
                ClientId = proposal.ClientId,
                InsurancePolicyNumber = insurance.InsurancePolicyNumber,
                Status = insurance.Status,
                PremiumAmount = insurance.PremiumAmount
            };
        }



        public async Task<InsuranceResponse> GetInsuranceByProposalIdAsync(int proposalId)
        {
            var insurance = await _context.Insurances.FirstOrDefaultAsync(i => i.ProposalId == proposalId);

            if (insurance == null)
                throw new Exception("Insurance not found.");

            return new InsuranceResponse
            {
                InsurancePolicyNumber = insurance.InsurancePolicyNumber,
                Status = insurance.Status,
                PremiumAmount = insurance.PremiumAmount
            };
        }

        public async Task<IEnumerable<ClientPolicyStatusDto>> GetClientPolicyStatusAsync(int clientId)
        {
            var proposals = await _context.Proposals
                .Include(p => p.Vehicle)
                .Include(p => p.InsuranceDetails)
                .Include(p => p.Insurance)
                .Where(p => p.ClientId == clientId)
                .ToListAsync();

            if (!proposals.Any())
                throw new Exception("No proposals found for the client.");

            var result = proposals.Select(p => new ClientPolicyStatusDto
            {
                ProposalId = p.ProposalId,
                VehicleNumber = p.Vehicle?.VehicleNumber,
                VehicleType = p.Vehicle?.VehicleType,
                InsuranceType = p.InsuranceType,
                ProposalCreatedAt = p.CreatedAt,
                ProposalStatus = p.Status,

                CalculatedPremium = p.InsuranceDetails?.CalculatedPremium,
                InsuranceStartDate = p.InsuranceDetails?.InsuranceStartDate,
                InsuranceSum = p.InsuranceDetails?.InsuranceSum,

                InsurancePolicyNumber = p.Insurance?.InsurancePolicyNumber,
                InsuranceStatus = p.Insurance?.Status,
                InsuranceCreatedAt = p.Insurance?.CreatedAt
            });

            return result;
        }



    }


}
