using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : ControllerBase
    {
        private readonly IInsuranceClaimService _claimService;
        private readonly IDocumentService _documentService;
        private readonly ILogger<ClaimController> _logger;

        public ClaimController(IInsuranceClaimService claimService, IDocumentService documentService, ILogger<ClaimController> logger)
        {
            _claimService = claimService;
            _documentService = documentService;
            _logger = logger;
        }

        [HttpPost("submit")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<CreateClaimResponse>> SubmitClaim([FromForm] CreateClaimRequest request)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Client ID not found.");

                var response = await _claimService.SubmitClaimAsync(request);
                _logger.LogInformation("Claim submitted with ClaimId: {ClaimId}", response.ClaimId);

                if (request.Documents != null)
                {
                    request.Documents.ClaimId = response.ClaimId;
                    await _documentService.SaveClaimDocumentsAsync(request.Documents);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error submitting claim: {Error}", ex.Message);
                return BadRequest($"Failed to submit claim: {ex.Message}");
            }
        }
        public ClaimController(IInsuranceClaimService claimService, ILogger<ClaimController> logger)
        {
            _claimService = claimService;
            _logger = logger;
        }

        // GET: api/Claim/client/5
        [HttpGet("client/{clientId}")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<IEnumerable<ClaimSummaryDto>>> GetClaimsForClient(int clientId)
        {
            try
            {
                var claims = await _claimService.GetClaimsForClient(clientId);
                return Ok(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching client claims.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Claim/all
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ClaimSummaryDto>>> GetAllClaims()
        {
            try
            {
                var claims = await _claimService.GetAllClaims();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all claims.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Claim/status/5?status=Approved
        [HttpPut("status/{claimId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateClaimStatus(int claimId, [FromQuery] string status)
        {
            try
            {
                var updated = await _claimService.UpdateClaimStatus(claimId, status);
                if (!updated)
                    return NotFound($"Claim with ID {claimId} not found.");

                return Ok($"Claim status updated to {status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating claim status.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
