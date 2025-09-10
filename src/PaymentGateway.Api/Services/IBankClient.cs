using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Services;

public interface IBankClient
{
    Task<PostPaymentResponse> ProcessPayment(PaymentRequest paymentRequest);
}
