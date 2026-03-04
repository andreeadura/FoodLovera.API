using FoodLovera.Core.Services;
using FoodLovera.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodLovera.API.Controllers;

[ApiController]
[Route("api/admin/restaurants")]
[Authorize(Roles = "Admin")]
public sealed class AdminRestaurantsController(IAdminRestaurantService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRestaurantRequestDTO request, CancellationToken ct)
    {
        var id = await service.CreateAsync(request.Name, request.CityId, request.ImageUrl, request.IsActive, ct);
        return Created($"/api/admin/restaurants/{id}", new { id });
    }

    [HttpDelete("{restaurantId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int restaurantId, CancellationToken ct)
    {
        await service.DeleteAsync(restaurantId, ct);
        return NoContent();
    }
}