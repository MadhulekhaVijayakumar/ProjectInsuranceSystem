using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Client")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<CreateVehicleResponse>> RegisterVehicle(CreateVehicleRequest request)
        {
            var clientId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value ?? "0");
            request.ClientId = clientId;
            var response = await _vehicleService.RegisterVehicle(request);
            return Ok(response);
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetMyVehicles()
        {
            var clientId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "ClientId")?.Value ?? "0");
            var vehicles = await _vehicleService.GetAllVehiclesByClient(clientId);
            return Ok(vehicles);
        }

        [HttpGet("{vehicleId}")]
        public async Task<ActionResult<VehicleDto>> GetVehicle(int vehicleId)
        {
            var vehicle = await _vehicleService.GetVehicleById(vehicleId);
            return Ok(vehicle);
        }
    }
}
