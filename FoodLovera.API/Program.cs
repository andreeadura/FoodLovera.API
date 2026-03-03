using FoodLovera.API.Ioc;
using FoodLovera.API.Middleware;
using FoodLovera.API.OpenApi;
using FoodLovera.Core.Contracts;
using FoodLovera.Infrastructure;
using FoodLovera.Infrastructure.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddProjectDependencies(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

builder.Services.AddHttpClient<IGeocodingService, NominatimGeocodingProviders>(http =>
{
    http.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
    http.DefaultRequestHeaders.UserAgent.ParseAdd("FoodLovera/1.0 (dev)");
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");

        var issuer = jwt["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing");
        var audience = jwt["Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing");
        var signingKey = jwt["SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey missing");

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddOpenApi(options =>
{
    options.AddScalarTransformers();               
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); 
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FoodLoveraDbContext>();
    await DevDatabaseSeeder.SeedAsync(db, CancellationToken.None);
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();           
    app.MapScalarApiReference();
}

app.Run();