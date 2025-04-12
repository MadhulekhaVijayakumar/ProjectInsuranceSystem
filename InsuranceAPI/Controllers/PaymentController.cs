using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // Ensure no method or property is named PaymentController
        [HttpPost("make-payment")]
        public async Task<IActionResult> MakePayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var paymentResponse = await _paymentService.MakePayment(request);
                return Ok(paymentResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred during payment processing." });
            }
        }
    }
}
