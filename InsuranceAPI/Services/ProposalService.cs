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
        private readonly ILogger<ProposalService> _logger;
        private readonly IDocumentService _documentService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProposalService(
            IRepository<int, Proposal> proposalRepo,
            IRepository<int, Vehicle> vehicleRepo,
            IRepository<int, InsuranceDetails> insuranceRepo,
            IPremiumCalculatorService premiumCalculatorService,
            IDocumentService documentService,
           ILogger<ProposalService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _proposalRepository = proposalRepo;
            _vehicleRepository = vehicleRepo;
            _insuranceDetailsRepository = insuranceRepo;
            _premiumCalculatorService = premiumCalculatorService;
            _documentService = documentService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateProposalResponse> SubmitProposalWithDetails(CreateProposalRequest request)
        {
            _logger.LogInformation("Starting proposal submission process.");

            var clientIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out int clientId))
            {
                _logger.LogWarning("Client ID missing or invalid in token.");
                throw new UnauthorizedAccessException("Client ID not found in token.");
            }

            _logger.LogInformation("Creating vehicle for ClientId: {ClientId}", clientId);
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
            _logger.LogInformation("Vehicle created with ID: {VehicleId}", createdVehicle.VehicleId);

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
            _logger.LogInformation("Proposal created with ID: {ProposalId}", createdProposal.ProposalId);

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

            var premium = _premiumCalculatorService.CalculatePremium(insuranceDetails, vehicle);
            insuranceDetails.CalculatedPremium = premium;

            _logger.LogInformation("Calculated premium: {Premium}", premium);
            await _insuranceDetailsRepository.Add(insuranceDetails);
            _logger.LogInformation("Insurance details saved for ProposalId: {ProposalId}", createdProposal.ProposalId);

            if (request.Documents != null)
            {
                try
                {
                    request.Documents.ProposalId = createdProposal.ProposalId;
                    await _documentService.SaveDocumentsAsync(request.Documents);
                    _logger.LogInformation("Documents saved successfully for ProposalId: {ProposalId}", createdProposal.ProposalId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Document upload failed for ProposalId: {ProposalId}", createdProposal.ProposalId);
                    throw; // Optional: rethrow if you want to stop the process
                }
            }

            _logger.LogInformation("Proposal submission completed for ProposalId: {ProposalId}", createdProposal.ProposalId);

            return new CreateProposalResponse
            {
                ProposalId = createdProposal.ProposalId,
                Status = createdProposal.Status,
                CalculatedPremium = premium
            };
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
