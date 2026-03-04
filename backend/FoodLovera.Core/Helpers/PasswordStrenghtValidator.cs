#nullable enable
using System.Text.RegularExpressions;

namespace FoodLovera.Core.Helpers;

public static class PasswordStrengthValidator
{
    
    public static IReadOnlyList<string> Validate(string? password, int minLength = 8)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required.");
            return errors;
        }

        if (password.Length < minLength)
            errors.Add($"Password must be at least {minLength} characters long.");

        if (!Regex.IsMatch(password, "[A-Z]"))
            errors.Add("Password must contain at least one uppercase letter.");

        if (!Regex.IsMatch(password, "[a-z]"))
            errors.Add("Password must contain at least one lowercase letter.");

        if (!Regex.IsMatch(password, "[0-9]"))
            errors.Add("Password must contain at least one digit.");


        if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            errors.Add("Password must contain at least one special character.");

        return errors;
    }
}