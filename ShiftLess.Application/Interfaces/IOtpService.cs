using System;
using System.Collections.Generic;
using System.Text;
namespace ShiftLess.Application.Interfaces;

public interface IOtpService
{
    string GenerateCode();

    string Hash(string phone, string purpose, string code);

    bool HashesMatch(string a, string b);

    bool ExposeDebugCode { get; }
}