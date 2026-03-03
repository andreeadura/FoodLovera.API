#nullable enable
using System.Security.Cryptography;
using System.Text;

namespace FoodLovera.Core.Helpers;

public static class VerificationCode
{
    public static string Generate6Digits()
    {
        var n = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return n.ToString("D6");
    }

    public static string Hash(string code)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(code));
        return Convert.ToHexString(bytes);
    }
}