using System.Text.Json;

using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Services;

public class BankClient(IHttpClientFactory httpClientFactory) : IBankClient
{
    private const string ClientUri = "http://localhost:8080/";

    public async Task<PostPaymentResponse> ProcessPayment(PaymentRequest paymentRequest)
    {
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(ClientUri);

        var clientRequest = new BankClientRequest
        {
            CardNumber = paymentRequest.CardNumber,
            ExpiryDate = $"{paymentRequest.ExpiryMonth:00}/{paymentRequest.ExpiryYear}",
            Currency = paymentRequest.Currency,
            Amount = paymentRequest.Amount,
            CVV = paymentRequest.CVV
        };
        var response = await httpClient.PostAsync("payments", new StringContent(JsonSerializer.Serialize(clientRequest)));
        var result = JsonSerializer.Deserialize<BankClientResponse>(response.Content.ReadAsStream()) 
            ?? new BankClientResponse {  Authorized = false };

        return new PostPaymentResponse
        {
            CardLastFourDigits = paymentRequest.CardNumber.Substring(11, 4),
            Currency = paymentRequest.Currency,
            Amount = paymentRequest.Amount,
            ExpiryMonth = paymentRequest.ExpiryMonth,
            ExpiryYear = paymentRequest.ExpiryYear,
            Id = Guid.NewGuid(),
            Status = result.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined
        };
    }
}
