using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Models;
using InsuranceAPI.Repositories;

namespace InsuranceAPI.Services
{
    public class ProposalService : IProposalService
    {
        private readonly IRepository<int, Proposal> _proposalRepository;
        private readonly IRepository<int, Vehicle> _vehicleRepository;
        private readonly IRepository<int, InsuranceDetails> _insuranceDetailsRepository;

        public ProposalService(
            IRepository<int, Proposal> proposalRepo,
            IRepository<int, Vehicle> vehicleRepo,
            IRepository<int, InsuranceDetails> insuranceRepo)
        {
            _proposalRepository = proposalRepo;
            _vehicleRepository = vehicleRepo;
            _insuranceDetailsRepository = insuranceRepo;
        }

        public async Task<CreateProposalResponse> SubmitProposalWithDetails(CreateProposalRequest request)
        {
            // 1. Create and save Vehicle
            var vehicle = new Vehicle
            {
                ClientId = request.ClientId,
                VehicleType = request.VehicleType,
                VehicleNumber = request.VehicleNumber,
                ChassisNumber = request.ChassisNumber,
                EngineNumber = request.EngineNumber,
                MakerName = request.MakerName,
                ModelName = request.ModelName,
                VehicleColor = request.VehicleColor,
                FuelType = request.FuelType,
                RegistrationDate = request.RegistrationDate,
                SeatCapacity = request.SeatCapacity
            };

            var createdVehicle = await _vehicleRepository.Add(vehicle);

            // 2. Create and save Proposal
            var proposal = new Proposal
            {
                ClientId = request.ClientId,
                VehicleId = createdVehicle.VehicleId,
                InsuranceType = request.InsuranceType,
                InsuranceValidUpto = request.InsuranceValidUpto,
                FitnessValidUpto = request.FitnessValidUpto,
                Status = "submitted",
                CreatedAt = DateTime.Now
            };

            var createdProposal = await _proposalRepository.Add(proposal);

            // 3. Create and save InsuranceDetails
            var insuranceDetails = new InsuranceDetails
            {
                ProposalId = createdProposal.ProposalId,
                VehicleId = createdVehicle.VehicleId,
                InsuranceStartDate = request.InsuranceStartDate,
                InsuranceSum = request.InsuranceSum,
                DamageInsurance = request.DamageInsurance,
                LiabilityOption = request.LiabilityOption,
                Plan = request.Plan
            };

            await _insuranceDetailsRepository.Add(insuranceDetails);

            return new CreateProposalResponse
            {
                ProposalId = createdProposal.ProposalId
            };
        }
    }


}
