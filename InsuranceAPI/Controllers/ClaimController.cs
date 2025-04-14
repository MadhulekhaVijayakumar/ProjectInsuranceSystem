using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsuranceClaimController : ControllerBase
    {
        private readonly IInsuranceClaimService _claimService;

        public InsuranceClaimController(IInsuranceClaimService claimService)
        {
            _claimService = claimService;
        }

        [Authorize(Roles = "Client")]
        [HttpPost("submit")]
        public async Task<ActionResult<CreateClaimResponse>> SubmitClaimWithDocuments([FromForm] CreateClaimRequest request)
        {
            try
            {
                var result = await _claimService.SubmitClaimWithDocumentsAsync(request, User);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new CreateClaimResponse { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Client")]
        [HttpGet("my-claims")]
        public async Task<ActionResult<IEnumerable<InsuranceClaim>>> GetMyClaims()
        {
            try
            {
                var claims = await _claimService.GetClaimsByClientAsync(User);
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<InsuranceClaim>>> GetAllClaims()
        {
            var claims = await _claimService.GetAllClaimsForAdminAsync();
            return Ok(claims);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-status/{claimId}")]
        public async Task<ActionResult<InsuranceClaim>> UpdateClaimStatus(int claimId, [FromQuery] string newStatus)
        {
            try
            {
                var updatedClaim = await _claimService.UpdateClaimStatusAsync(claimId, newStatus);
                return Ok(updatedClaim);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
