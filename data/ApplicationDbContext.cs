using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Room> Rooms { get; set; }

        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Participation> Participations { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // Deleting a room also deletes its bookings.
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

                // Many-to-many relationship between Room and Equipment.
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Equipment)
                .WithMany(e => e.Rooms);

               // One-to-many relationship between Booking and Participation.
            modelBuilder.Entity<Participation>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Participations)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }

}
