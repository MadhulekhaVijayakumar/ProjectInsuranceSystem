using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using System.Security.Claims;

namespace InsuranceAPI.Services
{
    public class ProposalService : IProposalService
    {
        private readonly IRepository<int, Proposal> _proposalRepository;
        private readonly IRepository<int, Vehicle> _vehicleRepository;
        private readonly IRepository<int, InsuranceDetails> _insuranceDetailsRepository;
        private readonly IPremiumCalculatorService _premiumCalculatorService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProposalService(
            IRepository<int, Proposal> proposalRepo,
            IRepository<int, Vehicle> vehicleRepo,
            IRepository<int, InsuranceDetails> insuranceRepo,
            IPremiumCalculatorService premiumCalculatorService,
            IHttpContextAccessor httpContextAccessor)
        {
            _proposalRepository = proposalRepo;
            _vehicleRepository = vehicleRepo;
            _insuranceDetailsRepository = insuranceRepo;
            _premiumCalculatorService = premiumCalculatorService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateProposalResponse> SubmitProposalWithDetails(CreateProposalRequest request)
        {
            // 1. Extract ClientId from JWT Token
            var clientIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out int clientId))
            {
                throw new UnauthorizedAccessException("Client ID not found in token.");
            }

            // 2. Create and Save Vehicle from request.Vehicle
            var vehicle = new Vehicle
            {
                ClientId = clientId,
                VehicleType = request.Vehicle.VehicleType,
                VehicleNumber = request.Vehicle.VehicleNumber,
                ChassisNumber = request.Vehicle.ChassisNumber,
                EngineNumber = request.Vehicle.EngineNumber,
                MakerName = request.Vehicle.MakerName,
                ModelName = request.Vehicle.ModelName,
                VehicleColor = request.Vehicle.VehicleColor,
                FuelType = request.Vehicle.FuelType,
                RegistrationDate = request.Vehicle.RegistrationDate,
                SeatCapacity = request.Vehicle.SeatCapacity
            };

            var createdVehicle = await _vehicleRepository.Add(vehicle);

            // 3. Create and Save Proposal from request.Proposal
            var proposal = new Proposal
            {
                ClientId = clientId,
                VehicleId = createdVehicle.VehicleId,
                InsuranceType = request.Proposal.InsuranceType,
                InsuranceValidUpto = request.Proposal.InsuranceValidUpto,
                FitnessValidUpto = request.Proposal.FitnessValidUpto,
                Status = "submitted",
                CreatedAt = DateTime.Now
            };

            var createdProposal = await _proposalRepository.Add(proposal);

            // 4. Create InsuranceDetails from request.InsuranceDetails
            var insuranceDetails = new InsuranceDetails
            {
                ProposalId = createdProposal.ProposalId,
                VehicleId = createdVehicle.VehicleId,
                InsuranceStartDate = request.InsuranceDetails.InsuranceStartDate,
                InsuranceSum = request.InsuranceDetails.InsuranceSum,
                DamageInsurance = request.InsuranceDetails.DamageInsurance,
                LiabilityOption = request.InsuranceDetails.LiabilityOption,
                Plan = request.InsuranceDetails.Plan
            };

            // 5. Calculate Premium
            try
            {
                var premium = _premiumCalculatorService.CalculatePremium(insuranceDetails, vehicle);
                insuranceDetails.CalculatedPremium = premium;

                // 6. Save InsuranceDetails
                await _insuranceDetailsRepository.Add(insuranceDetails);

                // 7. Return response
                return new CreateProposalResponse
                {
                    ProposalId = createdProposal.ProposalId,
                    Status = createdProposal.Status,
                    CalculatedPremium = premium
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculating premium: " + ex.Message);
            }
        }

        public async Task<IEnumerable<ProposalReviewDto>> GetSubmittedProposals()
        {
            var proposals = await _proposalRepository.GetAll();

            var submittedProposals = proposals
                .Where(p => p.Status == "submitted")
                .Select(p => new ProposalReviewDto
                {
                    ProposalId = p.ProposalId,
                    ClientName = p.Client?.Name ?? "Unknown",
                    VehicleNumber = p.Vehicle?.VehicleNumber ?? "N/A",
                    InsuranceType = p.InsuranceType,
                    InsuranceValidUpto = p.InsuranceValidUpto,
                    FitnessValidUpto = p.FitnessValidUpto,
                    Status = p.Status
                });

            return submittedProposals;
        }
        public async Task<bool> VerifyProposal(int proposalId, bool approve)
        {
            var proposal = await _proposalRepository.GetById(proposalId);
            if (proposal == null || proposal.Status != "submitted")
                return false;

            proposal.Status = approve ? "approved" : "rejected";
            await _proposalRepository.Update(proposalId, proposal);
            return true;
        }




    }
}
