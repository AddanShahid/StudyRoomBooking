using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.DAL
{
    // Applies the migrations on startup and fills an empty database with example data,
    // so the application has content the first time it is run.
    public static class DbInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();

            if (context.Rooms.Any())
            {
                return;
            }

            var rooms = new List<Room>
            {
                new() { Name = "Group Room 1", Location = "Pilestredet 35, 2nd floor", Capacity = 6, HasWhiteboard = true, HasScreen = true,
                        Description = "Bright room with a large screen, good for presentations." },
                new() { Name = "Group Room 2", Location = "Pilestredet 35, 2nd floor", Capacity = 4, HasWhiteboard = true,
                        Description = "Small room next to the library." },
                new() { Name = "Quiet Room", Location = "Pilestredet 46, 1st floor", Capacity = 2,
                        Description = "For focused work. Please keep your voices down." },
                new() { Name = "Project Lab", Location = "Pilestredet 52, 3rd floor", Capacity = 12, HasWhiteboard = true, HasScreen = true,
                        Description = "Large room for project groups, with two whiteboards." },
            };
            context.Rooms.AddRange(rooms);

            // Example bookings relative to today, so the overview is never empty.
            var today = DateTime.Today;
            context.Bookings.AddRange(
                new Booking { Room = rooms[0], StudentName = "Kari Nordmann", Subject = "ITPE3200", Topic = "Group meeting",
                              Date = today.AddDays(1), StartTime = new(9, 0), EndTime = new(11, 0) },
                new Booking { Room = rooms[0], StudentName = "Ola Hansen", Subject = "DATA1700", Topic = "Exam preparation",
                              Date = today.AddDays(1), StartTime = new(13, 0), EndTime = new(15, 0) },
                new Booking { Room = rooms[3], StudentName = "Aisha Khan", Subject = "ITPE3200", Topic = "Project work",
                              Date = today.AddDays(2), StartTime = new(10, 0), EndTime = new(14, 0) },
                new Booking { Room = rooms[2], StudentName = "Lars Berg", Subject = "DAPE1400",
                              Date = today.AddDays(3), StartTime = new(8, 0), EndTime = new(10, 0) });

            context.SaveChanges();
        }
    }
}
