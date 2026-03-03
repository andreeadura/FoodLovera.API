#nullable enable

using FoodLovera.Core.Services;
using FoodLovera.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodLovera.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
        => Ok(await _auth.RegisterAsync(request, ct));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
        => Ok(await _auth.LoginAsync(request, ct));

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDTO request, CancellationToken ct)
    {
        await _auth.VerifyEmailAsync(request, ct);
        return Ok();
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequestDTO request, CancellationToken ct)
    {
        await _auth.ResendVerificationCodeAsync(request.Email, ct);
        return Ok();
    }

}