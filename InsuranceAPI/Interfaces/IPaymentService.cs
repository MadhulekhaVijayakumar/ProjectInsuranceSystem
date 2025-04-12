using InsuranceAPI.Models.DTOs;

namespace InsuranceAPI.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> MakePayment(CreatePaymentRequest request);
    }
}
