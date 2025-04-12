using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class InsuranceController : ControllerBase
    {
        private readonly IInsuranceService _insuranceService;

        public InsuranceController(IInsuranceService insuranceService)
        {
            _insuranceService = insuranceService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("generate/{proposalId}")]
        public async Task<ActionResult<InsuranceResponse>> GenerateInsurance(int proposalId)
        {
            var insurance = await _insuranceService.GenerateInsuranceAsync(proposalId);
            return Ok(insurance);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("generate-quote/{proposalId}")]
        public async Task<ActionResult<InsuranceQuoteResponse>> GenerateQuote(int proposalId)
        {
            try
            {
                var quote = await _insuranceService.GenerateQuote(proposalId);
                return Ok(quote);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("by-proposal/{proposalId}")]
        public async Task<ActionResult<InsuranceResponse>> GetInsuranceByProposal(int proposalId)
        {
            var insurance = await _insuranceService.GetInsuranceByProposalIdAsync(proposalId);
            return Ok(insurance);
        }

        [Authorize(Roles = "Client")]
        [HttpGet("track")]
        public async Task<ActionResult<IEnumerable<ClientPolicyStatusDto>>> TrackMyPolicies()
        {
            var clientIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (clientIdClaim == null)
                return Unauthorized("Client ID not found in token.");

            if (!int.TryParse(clientIdClaim.Value, out int clientId))
                return Unauthorized("Invalid Client ID in token.");

            var result = await _insuranceService.GetClientPolicyStatusAsync(clientId);

            if (result == null || !result.Any())
                return NotFound("No policy records found.");

            return Ok(result);
        }




    }
}
