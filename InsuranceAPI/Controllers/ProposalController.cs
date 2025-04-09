using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProposalController : ControllerBase
    {
        private readonly IProposalService _proposalService;

        public ProposalController(IProposalService proposalService)
        {
            _proposalService = proposalService;
        }
        [HttpPost("submit")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<CreateProposalResponse>> SubmitProposal([FromBody] CreateProposalRequest request)
        {
            try
            {
                var response = await _proposalService.SubmitProposalWithDetails(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to submit proposal: {ex.Message}");
            }
        }
        [HttpGet("admin/submitted")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProposalReviewDto>>> GetSubmittedProposals()
        {
            var proposals = await _proposalService.GetSubmittedProposals();
            return Ok(proposals);
        }

        // 2. Approve or Reject a proposal
        [HttpPut("admin/verify/{proposalId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyProposal(int proposalId, [FromQuery] bool approve)
        {
            var success = await _proposalService.VerifyProposal(proposalId, approve);
            if (!success)
                return BadRequest("Invalid proposal ID or status is not 'submitted'");

            return Ok(new { message = approve ? "Proposal approved." : "Proposal rejected." });
        }
    }
}
