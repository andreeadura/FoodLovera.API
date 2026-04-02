using FoodLovera.Core.Contracts;
using FoodLovera.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodLovera.API.Controllers;

[ApiController]
[Route("api/lookups")]
public sealed class LookupsController : ControllerBase
{
    private readonly ICityRepository _cities;
    private readonly ICategoryRepository _categories;

    public LookupsController(
        ICityRepository cities,
        ICategoryRepository categories)
    {
        _cities = cities;
        _categories = categories;
    }

    [HttpGet("cities")]
    [ProducesResponseType(typeof(IReadOnlyList<PublicCityDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PublicCityDTO>>> GetCities(CancellationToken ct)
    {
        var result = await _cities.GetAllAsync(ct);

        return Ok(result.Select(x => new PublicCityDTO
        {
            Id = x.Id,
            Name = x.Name
        }).ToList());
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(IReadOnlyList<PublicCategoryDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PublicCategoryDTO>>> GetCategories(CancellationToken ct)
    {
        var result = await _categories.GetAllAsync(ct);

        return Ok(result.Select(x => new PublicCategoryDTO
        {
            Id = x.Id,
            Name = x.Name
        }).ToList());
    }
}