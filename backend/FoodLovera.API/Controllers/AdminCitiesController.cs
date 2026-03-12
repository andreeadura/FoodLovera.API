using FoodLovera.Core.Services;
using FoodLovera.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodLovera.API.Controllers;

[ApiController]
[Route("api/admin/cities")]
[Authorize(Roles = "Admin")]
public sealed class AdminCitiesController(IAdminCityService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var cities = await service.GetAllAsync(ct);
        return Ok(cities);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCityRequestDTO request, CancellationToken ct)
    {
        var id = await service.CreateAsync(request.Name, ct);
        return Created($"/api/admin/cities/{id}", new { id });
    }

    [HttpDelete("{cityId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int cityId, CancellationToken ct)
    {
        await service.DeleteAsync(cityId, ct);
        return NoContent();
    }
}