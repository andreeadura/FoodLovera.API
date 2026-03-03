using FoodLovera.Core.Abstractions;
using FoodLovera.Core.Services;
using FoodLovera.Infrastructure;
using FoodLovera.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FoodLovera.API.Ioc;

public static class DependencyContainer
{
    public static IServiceCollection AddProjectDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FoodLoveraDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("FoodLoveraDb")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FoodLoveraDbContext>());

        services.AddScoped<ISessionRepository, SessionRepository>();

        services.AddScoped<ISessionService, SessionService>();

        services.AddScoped<ISessionParticipantRepository, SessionParticipantRepository>();

        services.AddScoped<IRestaurantRepository, RestaurantRepository>();

        services.AddScoped<IParticipantRestaurantActionRepository, ParticipantRestaurantActionRepository>();

        services.AddScoped<ICityRepository, CityRepository>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();


        return services;
    }
}