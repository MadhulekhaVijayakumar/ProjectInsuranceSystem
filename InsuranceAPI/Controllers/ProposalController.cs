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
        public async Task<ActionResult<CreateProposalResponse>> SubmitProposal(CreateProposalRequest request)
        {
            var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClientId");
            if (clientIdClaim == null)
                return Unauthorized("Client ID not found in token.");

            int clientId = int.Parse(clientIdClaim.Value);

            request.ClientId = clientId; // Inject into request

            var response = await _proposalService.SubmitProposalWithDetails(request);
            return Ok(response);
        }

    }
}
