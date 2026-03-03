using System.Security.Claims;
using FoodLovera.Core.Contracts;
using FoodLovera.Core.Services;
using FoodLovera.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodLovera.API.Controllers;

[ApiController]
[Route("api/sessions")]
public sealed class SessionsController : ControllerBase
{
    private readonly ISessionService _sessions;
    private readonly IUserRepository _users;

    public SessionsController(ISessionService sessions, IUserRepository users)
    {
        _sessions = sessions;
        _users = users;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateSessionResponseDTO), StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateSessionResponseDTO>> Create(
        [FromBody] CreateSessionRequestDTO request,
        CancellationToken ct)
    {
        var result = await _sessions.CreateAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    // guest join 
    [HttpPost("{joinCode}/join")]
    [ProducesResponseType(typeof(JoinSessionResponseDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<JoinSessionResponseDTO>> Join(
        [FromRoute] string joinCode,
        [FromBody] JoinSessionRequestDTO request,
        CancellationToken ct)
    {
        var result = await _sessions.JoinAsync(joinCode, request, ct);
        return Ok(result);
    }

    // username joim
    [Authorize]
    [HttpPost("{joinCode}/join/me")]
    [ProducesResponseType(typeof(JoinSessionResponseDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<JoinSessionResponseDTO>> JoinMe(
        [FromRoute] string joinCode,
        CancellationToken ct)
    {
        var userIdRaw =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        if (!int.TryParse(userIdRaw, out var userId))
            return Unauthorized();

        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(user.Username))
            return BadRequest(new { message = "Username is not set for this account." });

        var request = new JoinSessionRequestDTO
        {
            DisplayName = user.Username
        };

        var result = await _sessions.JoinAsync(joinCode, request, ct);
        return Ok(result);
    }

    [HttpPost("{sessionId:int}/next")]
    [ProducesResponseType(typeof(NextResponseDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<NextResponseDTO>> Next(
        [FromRoute] int sessionId,
        [FromBody] NextRequestDTO request,
        CancellationToken ct)
    {
        var result = await _sessions.NextAsync(sessionId, request, ct);
        return Ok(result);
    }

    [HttpPost("{sessionId:int}/restaurants/{restaurantId:int}/like")]
    [ProducesResponseType(typeof(LikeResponseDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<LikeResponseDTO>> Like(
        [FromRoute] int sessionId,
        [FromRoute] int restaurantId,
        [FromBody] LikeRequestDTO request,
        CancellationToken ct)
    {
        var result = await _sessions.LikeAsync(sessionId, restaurantId, request, ct);
        return Ok(result);
    }
}