using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly IBankClient _mockBankClient;
    private readonly PaymentsRepository _mockPaymentsRepository;
    private readonly PaymentsController _paymentsController;
    public PaymentsControllerTests()
    {
        _mockBankClient = Substitute.For<IBankClient>();
        _mockPaymentsRepository = new PaymentsRepository();
        _paymentsController = new PaymentsController(_mockBankClient, _mockPaymentsRepository);
    }

    [Fact]
    public async Task Process_IsValid_ReturnsValue()
    {
        var input = new PaymentRequest
        {
            CardNumber = "4566123412341234",
            ExpiryMonth = 06,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 5500,
            CVV = "123"
        };

        var expectedResponse = new PostPaymentResponse
        {
            Id = new Guid("CDBC415D-FA2E-43C2-A00B-869E359B2373"),
            CardLastFourDigits = "1234",
            ExpiryMonth = 06,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 5500,
            Status = PaymentStatus.Authorized,
        };
        _mockBankClient.ProcessPayment(input).Returns(expectedResponse);

        var response = await _paymentsController.Process(input);

        Assert.Equal(expectedResponse, response);
        _mockPaymentsRepository.Get(expectedResponse.Id);
    }

    [Fact]
    public async Task Process_IsValid_SavesValue()
    {
        var input = new PaymentRequest
        {
            CardNumber = "4566123412341234",
            ExpiryMonth = 06,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 5500,
            CVV = "123"
        };

        var expectedResponse = new PostPaymentResponse
        {
            Id = new Guid("CDBC415D-FA2E-43C2-A00B-869E359B2373"),
            CardLastFourDigits = "1234",
            ExpiryMonth = 06,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 5500,
            Status = PaymentStatus.Authorized,
        };
        _mockBankClient.ProcessPayment(input).Returns(expectedResponse);

        await _paymentsController.Process(input);

        var savedValue = _mockPaymentsRepository.Get(expectedResponse.Id);
        Assert.Equal(expectedResponse, savedValue);
    }

    [Fact]
    public async Task Process_Invalid_ReturnsRejected()
    {
        _paymentsController.ModelState.AddModelError("model", "invalid");

        var response = await _paymentsController.Process(new PaymentRequest());

        Assert.Equal(PaymentStatus.Rejected, response.Status);
    }

    [Fact]
    public async Task Get_PaymentExists_ReturnsPayment()
    {
        var expectedResponse = new PostPaymentResponse
        {
            Id = new Guid("CDBC415D-FA2E-43C2-A00B-869E359B2373"),
            CardLastFourDigits = "1234",
            ExpiryMonth = 06,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 5500,
            Status = PaymentStatus.Authorized,
        };
        _mockPaymentsRepository.Add(expectedResponse);

        var response = await _paymentsController.GetPaymentAsync(expectedResponse.Id);
        var actualResponse = response.Result as OkObjectResult;

        Assert.NotNull(actualResponse);
        Assert.Equal(expectedResponse, actualResponse.Value);
    }
}
