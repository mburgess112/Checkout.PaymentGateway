using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models;

[CustomValidation(typeof(PaymentRequest), nameof(IsExpiryValid))]
public class PaymentRequest
{
    [Required]
    [StringLength(19, MinimumLength = 14)]
    [RegularExpression("\\d*")]
    public string CardNumber { get; set; }

    [Required]
    [Range(1, 12)]
    public int ExpiryMonth { get; set; }

    [Required]
    public int ExpiryYear { get; set; }

    // Ideally custom validator for this
    [Required]
    [AllowedValues("GBP", "EUR", "USD")]
    public string Currency { get; set; }

    [Required]
    public int Amount { get; set; }

    [Required]
    [StringLength(4, MinimumLength = 3)]
    public string CVV { get; set; }

    public static ValidationResult? IsExpiryValid(PaymentRequest paymentRequest)
    {
        // Ideally we'd abstract out the current-time dependency
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var expiryDate = new DateOnly(paymentRequest.ExpiryYear, paymentRequest.ExpiryMonth, 1);
        return expiryDate > currentDate ? 
            ValidationResult.Success : 
            new ValidationResult(ExpiryInvalidMessage);
    }

    public const string ExpiryInvalidMessage = "Expiry must be in the future";
}
