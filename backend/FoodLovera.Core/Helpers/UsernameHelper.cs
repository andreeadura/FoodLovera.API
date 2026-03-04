using System.Text.RegularExpressions;

namespace FoodLovera.Core.Helpers;

public static class UsernameHelper
{
    private static readonly Regex Allowed = new("^[a-z0-9]+$", RegexOptions.Compiled);

    public static string Normalize(string username)
        => (username ?? string.Empty).Trim().ToLowerInvariant();

    public static void ValidateOrThrow(string normalizedUsername, string paramName)
    {
        if (string.IsNullOrWhiteSpace(normalizedUsername))
            throw new ArgumentException("Username is required.", paramName);

        if (normalizedUsername.Length is < 3 or > 20)
            throw new ArgumentException("Username must be between 3 and 20 characters.", paramName);

        if (!Allowed.IsMatch(normalizedUsername))
            throw new ArgumentException("Username can contain only lowercase letters and digits.", paramName);
    }
}