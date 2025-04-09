using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        //for sign up purpose
        [HttpPost]

        public async Task<ActionResult<CreateClientResponse>> CreateClient(CreateClientRequest request)
        {
            try
            {
                var result = await _clientService.CreateClient(request);
                return Created("", result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("profile")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<ClientProfileResponse>> GetProfile()
        {
            var clientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (clientId == null) return Unauthorized();

            var result = await _clientService.GetClientProfile(int.Parse(clientId));
            return Ok(result);
        }

        [HttpPut("update-profile")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<ClientProfileResponse>> UpdateProfile([FromBody] UpdateClientRequest request)
        {
            try
            {
                // Extract Client ID from token
                var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (clientIdClaim == null)
                    return Unauthorized("Client ID not found in token");

                int clientId = int.Parse(clientIdClaim.Value);

                var updatedProfile = await _clientService.UpdateClientProfile(clientId, request);
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    


    }

}
