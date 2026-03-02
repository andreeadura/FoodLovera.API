using System.Globalization;
using System.Text;

namespace FoodLovera.Core.Helpers;

public static class CityNameNormalizer
{
   
    public static string ToCityKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var s = value.Trim().ToLowerInvariant();

        s = RemoveDiacritics(s);

     
        s = s.Replace('-', ' ')
             .Replace('_', ' ')
             .Replace('.', ' ');

        s = CollapseSpaces(s);

        
        s = s switch
        {
            "bucharest" => "bucuresti",
            "cluj" => "cluj napoca",
            _ => s
        };

        return s;
    }

    private static string RemoveDiacritics(string input)
    {
        var normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    private static string CollapseSpaces(string input)
        => string.Join(' ', input.Split(' ', StringSplitOptions.RemoveEmptyEntries));
}