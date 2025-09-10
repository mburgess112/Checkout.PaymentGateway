using System.ComponentModel.DataAnnotations;

using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Tests;

public class PaymentRequestTests
{
    [Fact]
    public void ExpiryIsNextMonth_ReturnsValid()
    {
        var futureDate = DateTime.Now.AddMonths(1);
        var input = new PaymentRequest
        {
            CardNumber = "4566123412341234",
            ExpiryMonth = futureDate.Month,
            ExpiryYear = futureDate.Year,
            Currency = "GBP",
            Amount = 5500,
            CVV = "123"
        };

        var results = new List<ValidationResult>();
        Validator.TryValidateObject(input, new ValidationContext(input), results);  

        Assert.Empty(results);
    }

    [Fact]
    public void ExpiryIsThisMonth_ReturnsValidationError()
    {
        var futureDate = DateTime.Now;
        var input = new PaymentRequest
        {
            CardNumber = "4566123412341234",
            ExpiryMonth = futureDate.Month,
            ExpiryYear = futureDate.Year,
            Currency = "GBP",
            Amount = 5500,
            CVV = "123"
        };

        var results = new List<ValidationResult>();
        Validator.TryValidateObject(input, new ValidationContext(input), results);

        Assert.Single(results);
        Assert.Equal(PaymentRequest.ExpiryInvalidMessage, results[0].ErrorMessage);
    }
}
