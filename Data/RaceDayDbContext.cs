using Microsoft.EntityFrameworkCore;
using RaceDayApi.Models;
using System.Reflection.Emit;

namespace RaceDayApi.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Organiser>()
                .HasIndex(o => o.UserID)
                .IsUnique();

            modelBuilder.Entity<Participant>()
                .HasIndex(p => p.UserID)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            modelBuilder.Entity<EventCategory>()
                .HasIndex(ec => new { ec.EventID, ec.CategoryID })
                .IsUnique();

            modelBuilder.Entity<Enrolment>()
                .HasIndex(e => new { e.ParticipantID, e.EventCategoryID })
                .IsUnique();

            modelBuilder.Entity<Result>()
                .HasIndex(r => r.EnrolmentID)
                .IsUnique();

            // Cascade rules (no cycles)
            modelBuilder.Entity<Organiser>()
                .HasOne(o => o.User)
                .WithOne(u => u.Organiser)
                .HasForeignKey<Organiser>(o => o.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithOne(u => u.Participant)
                .HasForeignKey<Participant>(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany(o => o.Events)
                .HasForeignKey(e => e.OrganiserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventCategory>()
                .HasOne(ec => ec.Event)
                .WithMany(e => e.EventCategories)
                .HasForeignKey(ec => ec.EventID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventCategory>()
                .HasOne(ec => ec.Category)
                .WithMany(c => c.EventCategories)
                .HasForeignKey(ec => ec.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Participant)
                .WithMany(p => p.Enrolments)
                .HasForeignKey(e => e.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.EventCategory)
                .WithMany(ec => ec.Enrolments)
                .HasForeignKey(e => e.EventCategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}