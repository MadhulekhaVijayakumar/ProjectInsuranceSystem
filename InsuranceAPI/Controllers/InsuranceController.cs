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
  
    public class InsuranceController : ControllerBase
    {
        private readonly IInsuranceService _insuranceService;
        private readonly IPolicyDocumentService _policyDocumentService;

        public InsuranceController(IInsuranceService insuranceService, IPolicyDocumentService policyDocumentService)
        {
            _insuranceService = insuranceService;
            _policyDocumentService = policyDocumentService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("Admin/GenerateInsurance/{proposalId}")]
        public async Task<ActionResult<InsuranceResponse>> GenerateInsurance(int proposalId)
        {
            try
            {
                var insurance = await _insuranceService.GenerateInsuranceAsync(proposalId);
                return Ok(insurance);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
            
        [Authorize(Roles = "Admin")]
        [HttpPost("Admin/Generatequote/{proposalId}")]
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
        [HttpGet("Admin/Getbyproposal/{proposalId}")]
        public async Task<ActionResult<InsuranceResponse>> GetInsuranceByProposal(int proposalId)
        {
            var insurance = await _insuranceService.GetInsuranceByProposalIdAsync(proposalId);
            return Ok(insurance);
        }

        [Authorize(Roles = "Client")]
        [HttpGet("Client/TrackStatus")]
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
        [HttpGet("Client/Download-policy/{insurancePolicyNumber}")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> DownloadPolicyDocument(string insurancePolicyNumber)
        {
            var insurance = await _insuranceService.GetInsuranceWithDetailsAsync(insurancePolicyNumber);
            if (insurance == null || insurance.Status != "active")
                return NotFound("Insurance policy not found or not active.");

            var pdfBytes = _policyDocumentService.GeneratePolicyDocument(insurance);

            return File(pdfBytes, "application/pdf", $"Policy_{insurancePolicyNumber}.pdf");
        }



    }
}
