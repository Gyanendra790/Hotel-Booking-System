using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Booking>()
                .HasOne(b => b.Guest)
                .WithMany(g => g.Bookings)
                .HasForeignKey(b => b.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Invoice>()
                .HasOne(i => i.Booking)
                .WithOne(b => b.Invoice)
                .HasForeignKey<Invoice>(i => i.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Rooms
            builder.Entity<Room>().HasData(
                new Room { RoomId = 1, RoomNumber = "101", RoomType = RoomType.Standard, Floor = 1, PricePerNight = 80, MaxOccupancy = 2, Description = "Cozy standard room with garden view", HasMiniBar = false },
                new Room { RoomId = 2, RoomNumber = "102", RoomType = RoomType.Standard, Floor = 1, PricePerNight = 80, MaxOccupancy = 2, Description = "Standard room with city view", HasMiniBar = false },
                new Room { RoomId = 3, RoomNumber = "201", RoomType = RoomType.Deluxe, Floor = 2, PricePerNight = 150, MaxOccupancy = 3, Description = "Deluxe room with king bed and balcony", HasMiniBar = true },
                new Room { RoomId = 4, RoomNumber = "202", RoomType = RoomType.Deluxe, Floor = 2, PricePerNight = 150, MaxOccupancy = 3, Description = "Deluxe room with sea view", HasMiniBar = true },
                new Room { RoomId = 5, RoomNumber = "301", RoomType = RoomType.Suite, Floor = 3, PricePerNight = 300, MaxOccupancy = 4, Description = "Luxurious suite with separate living area", HasMiniBar = true },
                new Room { RoomId = 6, RoomNumber = "302", RoomType = RoomType.Suite, Floor = 3, PricePerNight = 350, MaxOccupancy = 5, Description = "Presidential suite with panoramic views", HasMiniBar = true }
            );
        }
    }
}
