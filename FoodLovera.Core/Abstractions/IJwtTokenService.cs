#nullable enable
using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Abstractions;

public interface IJwtTokenService
{
    string CreateAccessToken(User user);
}