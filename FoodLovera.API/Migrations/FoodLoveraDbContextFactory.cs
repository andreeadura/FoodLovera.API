using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FoodLovera.Infrastructure;

public sealed class FoodLoveraDbContextFactory : IDesignTimeDbContextFactory<FoodLoveraDbContext>
{
    public FoodLoveraDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FoodLoveraDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=foodlovera;Username=postgres;Password=1234");

        return new FoodLoveraDbContext(optionsBuilder.Options);
    }
}