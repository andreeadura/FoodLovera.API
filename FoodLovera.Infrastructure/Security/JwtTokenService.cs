#nullable enable
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodLovera.Core.Abstractions;
using FoodLovera.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FoodLovera.Infrastructure.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    public JwtTokenService(IConfiguration config) => _config = config;

    public string CreateAccessToken(User user)
    {
        var jwt = _config.GetSection("Jwt");
        var issuer = jwt["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing");
        var audience = jwt["Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing");
        var key = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var expiresMinutes = int.TryParse(jwt["ExpiresMinutes"], out var m) ? m : 60;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // int user id
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}