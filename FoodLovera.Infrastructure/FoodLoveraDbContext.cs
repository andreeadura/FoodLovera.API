using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure;

using FoodLovera.Models.Entities;

public class FoodLoveraDbContext : DbContext
{
    public FoodLoveraDbContext(DbContextOptions<FoodLoveraDbContext> options)
        : base(options)
    {
    }

    public DbSet<Session> Sessions { get; set; } = default!;
    public DbSet<SessionParticipant> SessionParticipants { get; set; } = default!;
    public DbSet<City> Cities { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Restaurant> Restaurants { get; set; } = default!;
    public DbSet<RestaurantCategory> RestaurantCategories { get; set; } = default!;
    public DbSet<ParticipantRestaurantAction> ParticipantRestaurantActions { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RestaurantCategory>()
            .HasKey(rc => new { rc.RestaurantId, rc.CategoryId });

        modelBuilder.Entity<RestaurantCategory>()
            .HasOne(rc => rc.Restaurant)
            .WithMany(r => r.RestaurantCategories)
            .HasForeignKey(rc => rc.RestaurantId);

        modelBuilder.Entity<RestaurantCategory>()
            .HasOne(rc => rc.Category)
            .WithMany()
            .HasForeignKey(rc => rc.CategoryId);

        modelBuilder.Entity<ParticipantRestaurantAction>()
            .HasIndex(x => new { x.SessionId, x.ParticipantId, x.RestaurantId })
            .IsUnique();

    }

}
