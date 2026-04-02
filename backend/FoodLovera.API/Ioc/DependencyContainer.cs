using FoodLovera.Core.Contracts;
using FoodLovera.Core.Services;
using FoodLovera.Infrastructure;
using FoodLovera.Infrastructure.Email;
using FoodLovera.Infrastructure.Repositories;
using FoodLovera.Infrastructure.Security;
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
        services.AddScoped<ISessionParticipantRepository, SessionParticipantRepository>();
        services.AddScoped<ISessionRoundRepository, SessionRoundRepository>();
        services.AddScoped<IRestaurantRepository, RestaurantRepository>();
        services.AddScoped<IParticipantRestaurantActionRepository, ParticipantRestaurantActionRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IBannedEmailRepository, BannedEmailRepository>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IAdminCityService, AdminCityService>();
        services.AddScoped<IAdminRestaurantService, AdminRestaurantService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISessionService, SessionService>();

        return services;
    }
}