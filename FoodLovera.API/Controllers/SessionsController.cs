using FoodLovera.Core.Contracts;
using FoodLovera.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodLovera.API.Controllers;

[ApiController]
[Route("api/sessions")]
public sealed class SessionsController : ControllerBase
{
    private readonly ISessionService _sessions;

    public SessionsController(ISessionService sessions)
    {
        _sessions = sessions;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateSessionResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateSessionResponse>> Create(
        [FromBody] CreateSessionRequest request,
        CancellationToken ct)
    {
        var result = await _sessions.CreateAsync(request, ct);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("{joinCode}/join")]
    [ProducesResponseType(typeof(JoinSessionResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<JoinSessionResponse>> Join(
    [FromRoute] string joinCode,
    [FromBody] JoinSessionRequest request,
    CancellationToken ct)
    {
        var result = await _sessions.JoinAsync(joinCode, request, ct);
        return Ok(result);
    }
    [HttpPost("{sessionId:int}/next")]
    [ProducesResponseType(typeof(NextResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<NextResponse>> Next(
        [FromRoute] int sessionId,
        [FromBody] NextRequest request,
        CancellationToken ct)
    {
        var result = await _sessions.NextAsync(sessionId, request, ct);
        return Ok(result);
    }
    [HttpPost("{sessionId:int}/restaurants/{restaurantId:int}/like")]
    [ProducesResponseType(typeof(LikeResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LikeResponse>> Like(
    [FromRoute] int sessionId,
    [FromRoute] int restaurantId,
    [FromBody] LikeRequest request,
    CancellationToken ct)
    {
        var result = await _sessions.LikeAsync(sessionId, restaurantId, request, ct);
        return Ok(result);
    }
}