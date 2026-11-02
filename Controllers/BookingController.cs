
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Bookings.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        public async Task<IActionResult> Create(
            DateTime? date, TimeOnly? startTime, TimeOnly? endTime)
        {
            ViewBag.SearchDate = date;
            ViewBag.SearchStartTime = startTime;
            ViewBag.SearchEndTime = endTime;

            var booking = new Booking
            {
                Date = date ?? DateTime.Today,
                StartTime = startTime ?? new TimeOnly(8, 0),
                EndTime = endTime ?? new TimeOnly(9, 0)
            };

            ViewBag.Rooms = new List<Room>();

            if (date.HasValue && startTime.HasValue &&
                endTime.HasValue && startTime.Value < endTime.Value)
            {
                ViewBag.Rooms = await GetAvailableRooms(
                    date.Value, startTime.Value, endTime.Value);
            }

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,StudentName,RoomName,Subject,Topic,Date,StartTime,EndTime")]
            Booking booking)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Name == booking.RoomName);

            if (room == null)
            {
                ModelState.AddModelError("RoomName", "Velg et gyldig rom.");
            }
            else
            {
                if (booking.StartTime >= booking.EndTime)
                {
                    ModelState.AddModelError(
                        "EndTime", "Sluttid må være etter starttid.");
                }

                if (booking.StartTime < room.OpeningTime ||
                    booking.EndTime > room.ClosingTime)
                {
                    ModelState.AddModelError(
                        "RoomName",
                        $"Rommet er åpent fra {room.OpeningTime:HH:mm} til {room.ClosingTime:HH:mm}.");
                }

                bool isBooked = await _context.Bookings.AnyAsync(b =>
                    b.RoomName == booking.RoomName &&
                    b.Date.Date == booking.Date.Date &&
                    b.StartTime < booking.EndTime &&
                    b.EndTime > booking.StartTime);

                if (isBooked)
                {
                    ModelState.AddModelError(
                        "RoomName",
                        "Rommet er allerede booket på dette tidspunktet.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Rooms = await GetAvailableRooms(
                    booking.Date, booking.StartTime, booking.EndTime);

                ViewBag.SearchDate = booking.Date;
                ViewBag.SearchStartTime = booking.StartTime;
                ViewBag.SearchEndTime = booking.EndTime;

                return View(booking);
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<Room>> GetAvailableRooms(
            DateTime date, TimeOnly startTime, TimeOnly endTime)
        {
            var rooms = await _context.Rooms.ToListAsync();

            var bookedRoomNames = await _context.Bookings
                .Where(b =>
                    b.Date.Date == date.Date &&
                    b.StartTime < endTime &&
                    b.EndTime > startTime)
                .Select(b => b.RoomName)
                .ToListAsync();

            return rooms
                .Where(r =>
                    startTime >= r.OpeningTime &&
                    endTime <= r.ClosingTime &&
                    !bookedRoomNames.Contains(r.Name))
                .ToList();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,StudentName,RoomName,Subject,Topic,Date,StartTime,EndTime")]
            Booking booking)
        {
            if (id != booking.Id) return NotFound();

            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Name == booking.RoomName);

            if (room == null)
            {
                ModelState.AddModelError("RoomName", "Velg et gyldig rom.");
            }
            else
            {
                if (booking.StartTime >= booking.EndTime)
                {
                    ModelState.AddModelError(
                        "EndTime", "Sluttid må være etter starttid.");
                }

                if (booking.StartTime < room.OpeningTime ||
                    booking.EndTime > room.ClosingTime)
                {
                    ModelState.AddModelError(
                        "RoomName",
                        $"Rommet er åpent fra {room.OpeningTime:HH:mm} til {room.ClosingTime:HH:mm}.");
                }

                bool isBooked = await _context.Bookings.AnyAsync(b =>
                    b.Id != booking.Id &&
                    b.RoomName == booking.RoomName &&
                    b.Date.Date == booking.Date.Date &&
                    b.StartTime < booking.EndTime &&
                    b.EndTime > booking.StartTime);

                if (isBooked)
                {
                    ModelState.AddModelError(
                        "RoomName",
                        "Rommet er allerede booket på dette tidspunktet.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
