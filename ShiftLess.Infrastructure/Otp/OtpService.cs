using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using ShiftLess.Application.Interfaces;

namespace ShiftLess.Infrastructure.Otp;

public class OtpService : IOtpService
{
    private readonly byte[] _pepper;

    public OtpService(IOptions<OtpSettings> options)
    {
        var pepper = string.IsNullOrWhiteSpace(options.Value.Pepper)
            ? "shiftless-dev-otp-pepper"
            : options.Value.Pepper;

        _pepper = Encoding.UTF8.GetBytes(pepper);

        ExposeDebugCode = options.Value.ExposeCodeInResponse;
    }

    public bool ExposeDebugCode { get; }

    public string GenerateCode()
    {
        return RandomNumberGenerator
            .GetInt32(0, 1_000_000)
            .ToString("D6");
    }

    public string Hash(string phone, string purpose, string code)
    {
        using var hmac = new HMACSHA256(_pepper);

        var bytes = hmac.ComputeHash(
            Encoding.UTF8.GetBytes($"{phone}|{purpose}|{code}"));

        return Convert.ToHexString(bytes);
    }

    public bool HashesMatch(string a, string b)
    {
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(a),
            Encoding.UTF8.GetBytes(b));
    }
}