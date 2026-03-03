using FoodLovera.Core.Contracts;
using FoodLovera.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure;

public sealed class FoodLoveraDbContext : DbContext, IUnitOfWork
{
    public FoodLoveraDbContext(DbContextOptions<FoodLoveraDbContext> options)
        : base(options)
    {
    }

    Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken ct)
        => base.SaveChangesAsync(ct);

    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SessionParticipant> SessionParticipants => Set<SessionParticipant>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<RestaurantCategory> RestaurantCategories => Set<RestaurantCategory>();
    public DbSet<ParticipantRestaurantAction> ParticipantRestaurantActions => Set<ParticipantRestaurantAction>();

    public DbSet<SessionOutcome> SessionOutcomes => Set<SessionOutcome>();
    public DbSet<SessionOutcomeRestaurant> SessionOutcomeRestaurants => Set<SessionOutcomeRestaurant>();
    public DbSet<SessionCategory> SessionCategories => Set<SessionCategory>();
    public DbSet<User> Users => Set<User>();
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<BannedEmail> BannedEmails => Set<BannedEmail>();
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
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.Email)
                .HasMaxLength(256)
                .IsRequired();

            b.HasIndex(x => x.Email).IsUnique();

            b.Property(x => x.PasswordHash).IsRequired();

            b.Property(x => x.CreatedAt).IsRequired();

            b.Property(x => x.Role)
                .HasConversion<int>()
                .IsRequired();

            b.Property(x => x.Username)
                .HasMaxLength(20);

            b.HasIndex(x => x.Username).IsUnique();
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
            b.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();


            b.HasIndex(x => x.Name).IsUnique();
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
        modelBuilder.Entity<Session>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.SelectedCityId).IsRequired();
            b.Property(x => x.UseAllCategories).IsRequired();

            b.HasMany(x => x.SessionCategories)
                .WithOne(x => x.Session)
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Property(x => x.RequiredParticipants)
                .HasDefaultValue(2);
        });

        modelBuilder.Entity<SessionCategory>(b =>
        {
            b.HasKey(x => new { x.SessionId, x.CategoryId });

            b.HasOne(x => x.Session)
                .WithMany(x => x.SessionCategories)
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmailVerificationToken>(b =>
        {
            b.HasIndex(x => x.UserId).IsUnique(); 
            b.Property(x => x.CodeHash).IsRequired();
        });

        modelBuilder.Entity<PasswordResetToken>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.CodeHash).IsRequired();
            b.HasIndex(x => x.UserId).IsUnique(); 

            b.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BannedEmail>(b =>
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.EmailNormalized)
                .IsRequired()
                .HasMaxLength(256);

            b.HasIndex(x => x.EmailNormalized).IsUnique();

            b.Property(x => x.BannedAtUtc).IsRequired();
            b.Property(x => x.Reason).HasMaxLength(256);
        });
    }
}