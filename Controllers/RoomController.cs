
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(
            DateTime? date,
            TimeOnly? startTime,
            TimeOnly? endTime,
            int? equipmentId,
            int? minCapacity)
        {
            // Fix old rooms with missing opening hours
            _context.Rooms
                .Where(r => r.OpeningTime == TimeOnly.MinValue
                         && r.ClosingTime == TimeOnly.MinValue)
                .ExecuteUpdate(s => s
                    .SetProperty(r => r.OpeningTime, new TimeOnly(8, 0))
                    .SetProperty(r => r.ClosingTime, new TimeOnly(16, 0)));

            var rooms = _context.Rooms
                .Include(r => r.Equipments)
                .ToList();

            ViewBag.Equipments = _context.Equipments.ToList();

            ViewBag.SelectedDate = date;
            ViewBag.SelectedStartTime = startTime;
            ViewBag.SelectedEndTime = endTime;

            ViewBag.BookedRooms = new List<string>();

            if (date.HasValue && startTime.HasValue && endTime.HasValue)
            {
                var bookedRoomNames = _context.Bookings
                    .Where(b => b.Date.Date == date.Value.Date
                        && b.StartTime < endTime.Value
                        && b.EndTime > startTime.Value)
                    .Select(b => b.RoomName)
                    .ToList();

                ViewBag.BookedRooms = bookedRoomNames;
            }

            if (equipmentId.HasValue)
            {
                rooms = rooms
                    .Where(r => r.Equipments.Any(e => e.Id == equipmentId.Value))
                    .ToList();
            }

            if (minCapacity.HasValue)
            {
                rooms = rooms
                    .Where(r => r.Capacity >= minCapacity.Value)
                    .ToList();
            }

            return View(rooms);
        }

        public IActionResult Create()
        {
            ViewBag.Equipments = _context.Equipments.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room room, List<int> selectedEquipment)
        {
            if (ModelState.IsValid)
            {
                room.Equipments = _context.Equipments
                    .Where(e => selectedEquipment.Contains(e.Id))
                    .ToList();

                _context.Rooms.Add(room);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Equipments = _context.Equipments.ToList();
            return View(room);
        }
    }
}
