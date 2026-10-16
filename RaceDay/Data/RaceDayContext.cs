
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data;

public class RaceDayContext : DbContext
{
    public RaceDayContext(DbContextOptions<RaceDayContext> options)
        : base(options)
    {
    }

    public DbSet<Race> Races { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<EventCategory> EventCategories { get; set; }

    public DbSet<Event> Events { get; set; }

    public DbSet<Enrolment> Enrolments { get; set; }

    public DbSet<RaceResult> RaceResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Event>()
            .HasOne(e => e.EventCategory)
            .WithMany()
            .HasForeignKey(e => e.EventCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EventCategory>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrolment>()
            .HasOne(e => e.Event)
            .WithMany()
            .HasForeignKey(e => e.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Enrolment>()
            .HasIndex(e => new { e.UserId, e.EventId })
            .IsUnique();

        modelBuilder.Entity<RaceResult>()
            .HasOne(r => r.Enrolment)
            .WithOne()
            .HasForeignKey<RaceResult>(r => r.EnrolmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
