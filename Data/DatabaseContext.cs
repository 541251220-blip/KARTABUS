using KARTABUS.Models;
using Microsoft.EntityFrameworkCore;

namespace KARTABUS.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Passenger> Passengers => Set<Passenger>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Bus> Buses => Set<Bus>();
        public DbSet<Route> Routes => Set<Route>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<Seat> Seats => Set<Seat>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=kartabus.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Passenger>("Passenger")
                .HasValue<Admin>("Admin");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.BookingCode)
                .IsUnique();

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.BookingId)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.ScheduleId, b.SeatId })
                .IsUnique();

            modelBuilder.Entity<Bus>()
                .HasMany<Seat>()
                .WithOne(s => s.Bus)
                .HasForeignKey(s => s.BusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Bus)
                .WithMany()
                .HasForeignKey(s => s.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Route)
                .WithMany()
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Passenger)
                .WithMany()
                .HasForeignKey(b => b.PassengerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Schedule)
                .WithMany()
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Seat)
                .WithMany()
                .HasForeignKey(b => b.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne()
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
