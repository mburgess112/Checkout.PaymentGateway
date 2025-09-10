using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Services;

public class BankClientResponse
{
    public bool Authorized { get; set; }

    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }
}
