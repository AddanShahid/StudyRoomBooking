
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;
using StudyRoomBooking.ViewModels;

namespace StudyRoomBooking.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Room
        public IActionResult Index(
            DateTime? date,
            TimeOnly? startTime,
            TimeOnly? endTime,
            int? equipmentId,
            int? minCapacity)
        {
            // Set default opening hours for old rooms
            _context.Rooms
                .Where(r => r.OpeningTime == TimeOnly.MinValue
                         && r.ClosingTime == TimeOnly.MinValue)
                .ExecuteUpdate(s => s
                    .SetProperty(r => r.OpeningTime, new TimeOnly(8, 0))
                    .SetProperty(r => r.ClosingTime, new TimeOnly(16, 0)));

            var rooms = _context.Rooms
                .Include(r => r.Equipment)
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
                    .Select(b => b.Room != null ? b.Room.Name : "")
                    .ToList();

                ViewBag.BookedRooms = bookedRoomNames;
            }

            if (equipmentId.HasValue)
            {
                rooms = rooms
                    .Where(r => r.Equipment.Any(e => e.Id == equipmentId.Value))
                    .ToList();
            }

            if (minCapacity.HasValue)
            {
                rooms = rooms
                    .Where(r => r.Capacity >= minCapacity.Value)
                    .ToList();
            }

            // Convert Room models to RoomViewModels
            var roomViewModels = rooms.Select(r => new RoomViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Location = r.Location,
                Capacity = r.Capacity,
                Description = r.Description,
                OpeningTime = r.OpeningTime,
                ClosingTime = r.ClosingTime,
                Equipments = r.Equipment
            }).ToList();

            return View(roomViewModels);
        }

        // GET: Room/Create
        public IActionResult Create()
        {
            ViewBag.Equipments = _context.Equipments.ToList();
            return View();
        }

        // POST: Room/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room room, List<int> selectedEquipment)
        {
            if (ModelState.IsValid)
            {
                selectedEquipment ??= new List<int>();

                room.Equipment = _context.Equipments
                    .Where(e => selectedEquipment.Contains(e.Id))
                    .ToList();

                _context.Rooms.Add(room);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Equipments = _context.Equipments.ToList();
            return View(room);
        }

        // GET: Room/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var room = _context.Rooms
                .Include(r => r.Equipment)
                .FirstOrDefault(r => r.Id == id);

            if (room == null)
                return NotFound();

            ViewBag.Equipments = _context.Equipments.ToList();

            return View(room);
        }

        // POST: Room/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Room room, List<int> selectedEquipment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Equipments = _context.Equipments.ToList();
                return View(room);
            }

            var existingRoom = _context.Rooms
                .Include(r => r.Equipment)
                .FirstOrDefault(r => r.Id == room.Id);

            if (existingRoom == null)
                return NotFound();

            existingRoom.Name = room.Name;
            existingRoom.Location = room.Location;
            existingRoom.Capacity = room.Capacity;
            existingRoom.Description = room.Description;
            existingRoom.OpeningTime = room.OpeningTime;
            existingRoom.ClosingTime = room.ClosingTime;

            selectedEquipment ??= new List<int>();

            existingRoom.Equipment = _context.Equipments
                .Where(e => selectedEquipment.Contains(e.Id))
                .ToList();

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Room/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var room = _context.Rooms
                .Include(r => r.Equipment)
                .FirstOrDefault(r => r.Id == id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        // POST: Room/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var room = _context.Rooms
                .Include(r => r.Equipment)
                .FirstOrDefault(r => r.Id == id);

            if (room == null)
                return NotFound();

            // Remove equipment relationships first
            room.Equipment.Clear();

            _context.Rooms.Remove(room);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
