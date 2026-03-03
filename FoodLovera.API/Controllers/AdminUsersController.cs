#nullable enable
using System.Security.Claims;
using FoodLovera.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodLovera.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")] 
public sealed class AdminUsersController(IAdminUserService adminUsers) : ControllerBase
{
   
    [HttpDelete("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromRoute] int userId, CancellationToken ct)
    {
        var adminIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(adminIdStr, out var adminId))
            return Unauthorized();

        await adminUsers.DeleteUserAsync(userId, adminId, ct);
        return NoContent();
    }
}