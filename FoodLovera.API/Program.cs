using FoodLovera.API.Ioc;
using FoodLovera.Core.Abstractions;
using FoodLovera.Infrastructure;
using FoodLovera.Infrastructure.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.AddOpenApi();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProjectDependencies(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<FoodLovera.API.Middleware.ApiExceptionHandler>();
builder.Services.AddHttpClient<IGeocodingService, NominatimGeocodingProviders>(http =>

{
    http.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
    http.DefaultRequestHeaders.UserAgent.ParseAdd("FoodLovera/1.0 (dev)");
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SigningKey"]!)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FoodLoveraDbContext>();
    await DevDatabaseSeeder.SeedAsync(db, CancellationToken.None);
}

if (app.Environment.IsDevelopment())
{
  
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();