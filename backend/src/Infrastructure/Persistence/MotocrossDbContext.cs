using Microsoft.EntityFrameworkCore;
using Motocross.Domain.Entities;
using Motocross.Domain.Enums;
using Motocross.Domain.ValueObjects;

namespace Motocross.Infrastructure.Persistence;

public class MotocrossDbContext : DbContext
{
    public MotocrossDbContext(DbContextOptions<MotocrossDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<TrackingPoint> TrackingPoints => Set<TrackingPoint>();
    public DbSet<Lap> Laps => Set<Lap>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(320);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.DisplayName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            entity.HasMany(u => u.Sessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Session configuration
        modelBuilder.Entity<Session>(entity =>
        {
            entity.ToTable("Sessions");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(e => e.UserId)
                .IsRequired(false);

            // Value object - Coordinate
            entity.OwnsOne(e => e.StartFinishLine, coord =>
            {
                coord.Property(c => c.Latitude).HasColumnName("StartFinishLineLatitude");
                coord.Property(c => c.Longitude).HasColumnName("StartFinishLineLongitude");
            });

            entity.Property(e => e.StartFinishLineRadius)
                .HasDefaultValue(20);

            // Relationships
            entity.HasMany(s => s.TrackingPoints)
                .WithOne(tp => tp.Session)
                .HasForeignKey(tp => tp.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.Laps)
                .WithOne(l => l.Session)
                .HasForeignKey(l => l.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartTime);
            entity.HasIndex(e => e.UserId);
        });

        // TrackingPoint configuration
        modelBuilder.Entity<TrackingPoint>(entity =>
        {
            entity.ToTable("TrackingPoints");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Type)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Value objects
            entity.OwnsOne(e => e.Coordinate, coord =>
            {
                coord.Property(c => c.Latitude).HasColumnName("Latitude");
                coord.Property(c => c.Longitude).HasColumnName("Longitude");
            });

            entity.OwnsOne(e => e.Speed, speed =>
            {
                speed.Property(s => s.KilometersPerHour).HasColumnName("SpeedKmh");
            });

            // Indexes
            entity.HasIndex(e => e.SessionId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => new { e.SessionId, e.Timestamp });
        });

        // Lap configuration
        modelBuilder.Entity<Lap>(entity =>
        {
            entity.ToTable("Laps");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.LapNumber).IsRequired();

            // Value objects
            entity.OwnsOne(e => e.Duration, duration =>
            {
                duration.Property(d => d.Value).HasColumnName("Duration");
            });

            entity.OwnsOne(e => e.AverageSpeed, speed =>
            {
                speed.Property(s => s.KilometersPerHour).HasColumnName("AverageSpeedKmh");
            });

            entity.OwnsOne(e => e.MaxSpeed, speed =>
            {
                speed.Property(s => s.KilometersPerHour).HasColumnName("MaxSpeedKmh");
            });

            // Indexes
            entity.HasIndex(e => e.SessionId);
            entity.HasIndex(e => new { e.SessionId, e.LapNumber });
        });
    }
}
