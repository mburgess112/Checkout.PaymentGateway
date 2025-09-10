using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IBankClient bankClient, PaymentsRepository paymentsRepository) : ControllerBase
{
    private readonly IBankClient _bankClient = bankClient;
    private readonly PaymentsRepository _paymentsRepository = paymentsRepository;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = _paymentsRepository.Get(id);

        return new OkObjectResult(payment);
    }

    [HttpPost]
    public async Task<PostPaymentResponse> Process(PaymentRequest paymentRequest)
    {
        if (!ModelState.IsValid)
        {
            return new PostPaymentResponse
            {
                Status = PaymentStatus.Rejected
            };
        }
        var result = await _bankClient.ProcessPayment(paymentRequest);
        _paymentsRepository.Add(result);
        return result;
    }
}
