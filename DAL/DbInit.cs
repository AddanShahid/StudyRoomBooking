
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.DAL
{
    public static class DbInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();

            // Create example rooms if the database has none.
            if (!context.Rooms.Any())
            {
                var rooms = new List<Room>
                {
                    new() { Name = "Group Room 1", Location = "Pilestredet 35, 2nd floor", Capacity = 6, HasWhiteboard = true, HasScreen = true,
                        Description = "Bright room with a large screen, good for presentations." },
                    new() { Name = "Group Room 2", Location = "Pilestredet 35, 2nd floor", Capacity = 4, HasWhiteboard = true,
                        Description = "Small room next to the library." },
                    new() { Name = "Quiet Room", Location = "Pilestredet 46, 1st floor", Capacity = 2,
                        Description = "For focused work. Please keep your voices down." },
                    new() { Name = "Project Lab", Location = "Pilestredet 52, 3rd floor", Capacity = 12, HasWhiteboard = true, HasScreen = true,
                        Description = "Large room for project groups, with two whiteboards." }
                };

                context.Rooms.AddRange(rooms);
                context.SaveChanges();
            }

            // Load existing rooms and their equipment.
            var existingRooms = context.Rooms
                .Include(r => r.Equipment)
                .ToList();

            // Add equipment if it does not already exist.
            var equipmentNames = new[] { "Projector", "Whiteboard markers", "HDMI cable" };

            foreach (var name in equipmentNames)
            {
                if (!context.Equipments.Any(e => e.Name == name))
                {
                    context.Equipments.Add(new Equipment
                    {
                        Name = name,
                        Description = name == "Projector"
                            ? "Projector for presentations."
                            : name == "Whiteboard markers"
                                ? "Markers for whiteboards."
                                : "Cable for connecting a laptop to a screen."
                    });
                }
            }

            context.SaveChanges();

            // Find the equipment in the database.
            var projector = context.Equipments.First(e => e.Name == "Projector");
            var markers = context.Equipments.First(e => e.Name == "Whiteboard markers");
            var hdmi = context.Equipments.First(e => e.Name == "HDMI cable");

            // Assign equipment to rooms without creating duplicate relationships.
            var room1 = existingRooms.FirstOrDefault(r => r.Name == "Group Room 1");
            var room2 = existingRooms.FirstOrDefault(r => r.Name == "Group Room 2");
            var room4 = existingRooms.FirstOrDefault(r => r.Name == "Project Lab");

            if (room1 != null)
            {
                if (!room1.Equipment.Any(e => e.Id == projector.Id))
                    room1.Equipment.Add(projector);

                if (!room1.Equipment.Any(e => e.Id == markers.Id))
                    room1.Equipment.Add(markers);
            }

            if (room2 != null && !room2.Equipment.Any(e => e.Id == markers.Id))
                room2.Equipment.Add(markers);

            if (room4 != null)
            {
                if (!room4.Equipment.Any(e => e.Id == projector.Id))
                    room4.Equipment.Add(projector);

                if (!room4.Equipment.Any(e => e.Id == hdmi.Id))
                    room4.Equipment.Add(hdmi);
            }

            // Add example participants if the example bookings exist.
            var bookings = context.Bookings
                .Include(b => b.Participations)
                .ToList();

            var kariBooking = bookings.FirstOrDefault(b => b.StudentName == "Kari Nordmann");
            var olaBooking = bookings.FirstOrDefault(b => b.StudentName == "Ola Hansen");

            if (kariBooking != null && !kariBooking.Participations.Any())
            {
                kariBooking.Participations.Add(new Participation
                {
                    StudentName = "Student 1",
                    Message = "Looking forward to studying together!"
                });
            }

            if (olaBooking != null && !olaBooking.Participations.Any())
            {
                olaBooking.Participations.Add(new Participation
                {
                    StudentName = "Student 2",
                    Message = "I would like to join this study session."
                });
            }

            context.SaveChanges();
        }
    }
}
