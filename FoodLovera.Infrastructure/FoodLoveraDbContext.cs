using FoodLovera.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure;

public sealed class FoodLoveraDbContext : DbContext
{
    public FoodLoveraDbContext(DbContextOptions<FoodLoveraDbContext> options)
        : base(options)
    {
    }

    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SessionParticipant> SessionParticipants => Set<SessionParticipant>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<RestaurantCategory> RestaurantCategories => Set<RestaurantCategory>();
    public DbSet<ParticipantRestaurantAction> ParticipantRestaurantActions => Set<ParticipantRestaurantAction>();

    public DbSet<SessionOutcome> SessionOutcomes => Set<SessionOutcome>();
    public DbSet<SessionOutcomeRestaurant> SessionOutcomeRestaurants => Set<SessionOutcomeRestaurant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

     
        modelBuilder.Entity<RestaurantCategory>(b =>
        {
            b.HasKey(x => new { x.RestaurantId, x.CategoryId });

            b.HasOne(x => x.Restaurant)
                .WithMany(r => r.RestaurantCategories)
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Restaurant>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.ImageUrl).IsRequired();
            b.Property(x => x.IsActive).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();

            b.HasOne(x => x.City)
                .WithMany()
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

      
        modelBuilder.Entity<Category>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<City>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<ParticipantRestaurantAction>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.ActionType).IsRequired();

            b.HasIndex(x => new { x.SessionId, x.ParticipantId, x.RestaurantId })
                .IsUnique();

            b.HasOne<Session>()
                .WithMany()
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<SessionParticipant>()
                .WithMany()
                .HasForeignKey(x => x.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Restaurant>()
                .WithMany()
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

     
        modelBuilder.Entity<SessionOutcome>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.Reason).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();

            b.HasOne(x => x.Session)
                .WithMany()
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Restaurants)
                .WithOne(x => x.Outcome)
                .HasForeignKey(x => x.OutcomeId)
                .OnDelete(DeleteBehavior.Cascade);

    
        });

        modelBuilder.Entity<SessionOutcomeRestaurant>(b =>
        {
            b.HasKey(x => new { x.OutcomeId, x.RestaurantId });

            b.Property(x => x.LikeCount).IsRequired();

            b.HasOne(x => x.Outcome)
                .WithMany(x => x.Restaurants)
                .HasForeignKey(x => x.OutcomeId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Restaurant)
                .WithMany()
                .HasForeignKey(x => x.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}