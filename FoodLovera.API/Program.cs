using FoodLovera.API.Ioc;
using FoodLovera.Core.Abstractions;
using FoodLovera.Infrastructure;
using FoodLovera.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.AddOpenApi();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProjectDependencies(builder.Configuration);
builder.Services.AddHttpClient<IGeocodingService, NominatimGeocodingService>(http =>
{
    http.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
    http.DefaultRequestHeaders.UserAgent.ParseAdd("FoodLovera/1.0 (dev)");
});

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();