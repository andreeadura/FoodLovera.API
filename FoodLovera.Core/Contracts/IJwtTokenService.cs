#nullable enable
using FoodLovera;
using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Contracts;

public interface IJwtTokenService
{
    string CreateAccessToken(User user);
}