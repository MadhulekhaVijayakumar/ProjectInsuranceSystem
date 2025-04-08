using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Models;

namespace InsuranceAPI.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IRepository<int, Vehicle> _vehicleRepo;

        public VehicleService(IRepository<int, Vehicle> vehicleRepo)
        {
            _vehicleRepo = vehicleRepo;
        }

        public async Task<CreateVehicleResponse> RegisterVehicle(CreateVehicleRequest request)
        {
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

            var result = await _vehicleRepo.Add(vehicle);
            return new CreateVehicleResponse { VehicleId = result.VehicleId };
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesByClient(int clientId)
        {
            var vehicles = await _vehicleRepo.GetAll();
            return vehicles
                .Where(v => v.ClientId == clientId)
                .Select(v => new VehicleDto
                {
                    VehicleId = v.VehicleId,
                    ClientId = v.ClientId,
                    VechileType = v.VehicleType,
                    VehicleNumber = v.VehicleNumber,
                    ChassisNumber = v.ChassisNumber,
                    EngineNumber = v.EngineNumber,
                    MakerName = v.MakerName,
                    ModelName = v.ModelName,
                    VehicleColor = v.VehicleColor,
                    FuelType = v.FuelType,
                    RegistrationDate = v.RegistrationDate,
                    SeatCapacity = v.SeatCapacity
                });
        }

        public async Task<VehicleDto> GetVehicleById(int vehicleId)
        {
            var v = await _vehicleRepo.GetById(vehicleId);
            return new VehicleDto
            {
                VehicleId = v.VehicleId,
                ClientId = v.ClientId,
                VechileType= v.VehicleType,
                VehicleNumber = v.VehicleNumber,
                ChassisNumber = v.ChassisNumber,
                EngineNumber = v.EngineNumber,
                MakerName = v.MakerName,
                ModelName = v.ModelName,
                VehicleColor = v.VehicleColor,
                FuelType = v.FuelType,
                RegistrationDate = v.RegistrationDate,
                SeatCapacity = v.SeatCapacity
            };
        }
    }

}
