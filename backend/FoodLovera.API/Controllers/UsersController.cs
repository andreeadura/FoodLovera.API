using FoodLovera.Core.Services;
using FoodLovera.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodLovera.API.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserService users) : ControllerBase
{
    private readonly IUserService _users = users;

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<MyProfileResponseDTO>> GetMe(CancellationToken ct)
    {
        var userIdRaw =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        if (!int.TryParse(userIdRaw, out var userId))
            return Unauthorized();

        var profile = await _users.GetMyProfileAsync(userId, ct);
        return Ok(profile);
    }

    [Authorize]
    [HttpPut("me/username")]
    public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameRequestDTO request, CancellationToken ct)
    {
        var userIdRaw =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        if (!int.TryParse(userIdRaw, out var userId))
            return Unauthorized();

        await _users.ChangeUsernameAsync(userId, request, ct);
        return NoContent();
    }
}