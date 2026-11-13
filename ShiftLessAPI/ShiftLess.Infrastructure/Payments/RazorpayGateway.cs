using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ShiftLess.Application.Common.Exceptions;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Infrastructure.Payments;

public class RazorpayGateway : IPaymentGateway
{
    private readonly RazorpaySettings _settings;
    private readonly HttpClient _httpClient;

    public RazorpayGateway(
        IOptions<RazorpaySettings> options,
        HttpClient httpClient)
    {
        _settings = options.Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.razorpay.com/v1/");

        var authBytes = Encoding.UTF8.GetBytes($"{_settings.KeyId}:{_settings.KeySecret}");

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
    }

    public async Task<RazorpayOrderResult> CreateOrderAsync(decimal amount, string receipt)
    {
        var amountInPaise = (int)(amount * 100);

        var payload = new
        {
            amount = amountInPaise,
            currency = "INR",
            receipt,
            payment_capture = 1
        };

        var response = await _httpClient.PostAsJsonAsync("orders", payload);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            string reason;

            try
            {
                var errorJson = JsonSerializer.Deserialize<JsonElement>(responseBody);

                reason = errorJson
                    .GetProperty("error")
                    .GetProperty("description")
                    .GetString() ?? responseBody;
            }
            catch
            {
                reason = responseBody;
            }

            throw new BadRequestException($"Razorpay order creation failed: {reason}");
        }

        var json = JsonSerializer.Deserialize<JsonElement>(responseBody);

        return new RazorpayOrderResult
        {
            OrderId = json.GetProperty("id").GetString()!,
            Currency = json.GetProperty("currency").GetString()!,
            AmountInPaise = amountInPaise
        };
    }

    public bool VerifySignature(string orderId, string paymentId, string signature)
    {
        var payload = $"{orderId}|{paymentId}";

        var keyBytes = Encoding.UTF8.GetBytes(_settings.KeySecret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);

        var hash = hmac.ComputeHash(payloadBytes);

        var computedSignature = Convert.ToHexString(hash).ToLowerInvariant();

        return computedSignature == signature.ToLowerInvariant();
    }

    public string GetPublicKeyId() => _settings.KeyId;
}