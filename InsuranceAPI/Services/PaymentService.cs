using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models.DTOs;
using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace InsuranceAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly InsuranceManagementContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(InsuranceManagementContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<PaymentResponse> MakePayment(CreatePaymentRequest request)
        {
            try
            {
                // Step 1: Create a new Payment object with the details from the request
                var payment = new Payment
                {
                    ProposalId = request.ProposalId,
                    AmountPaid = request.AmountPaid,
                    PaymentDate = DateTime.Now,
                    PaymentMode = request.PaymentMode,
                    TransactionStatus = "success" // Set transaction status to success
                };

                // Step 2: Add the payment to the Payments table
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync(); // Save the changes in the database

                // Step 3: Get the generated PaymentId (since it's an identity column, it is auto-generated)
                var paymentId = payment.PaymentId;

                // Log the payment success
                _logger.LogInformation($"✅ Payment successful. ID: {paymentId}, ProposalId: {request.ProposalId}");

                // Step 4: Return the response object with payment details
                return new PaymentResponse
                {
                    PaymentId = paymentId,
                    ProposalId = request.ProposalId,
                    AmountPaid = request.AmountPaid,
                    PaymentDate = payment.PaymentDate,
                    PaymentMode = request.PaymentMode,
                    TransactionStatus = "success"
                };
            }
            catch (Exception ex)
            {
                // Log the error if payment fails
                _logger.LogError($"❌ Payment failed for ProposalId: {request.ProposalId} - {ex.Message}");
                throw new Exception("An unexpected error occurred during payment processing.", ex);
            }
        }

    }

}
