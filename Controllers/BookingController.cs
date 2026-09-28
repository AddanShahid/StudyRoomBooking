using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            ApplicationDbContext context,
            ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Henter alle bookinger.");

                var bookings = await _context.Bookings.ToListAsync();

                _logger.LogInformation(
                    "Hentet {Count} bookinger.",
                    bookings.Count);

                return View(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Feil ved henting av bookinger.");

                return Problem(
                    "Det oppstod en feil ved henting av bookingene.");
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning(
                    "Details ble kalt uten booking-ID.");

                return NotFound();
            }

            try
            {
                _logger.LogInformation(
                    "Henter booking med ID {BookingId}.",
                    id);

                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (booking == null)
                {
                    _logger.LogWarning(
                        "Fant ikke booking med ID {BookingId}.",
                        id);

                    return NotFound();
                }

                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Feil ved henting av booking med ID {BookingId}.",
                    id);

                return Problem(
                    "Det oppstod en feil ved henting av bookingen.");
            }
        }

        public IActionResult Create()
        {
            _logger.LogInformation(
                "Create-siden ble åpnet.");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,StudentName,RoomName,Subject,Topic,Date,StartTime,EndTime")]
            Booking booking)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Forsøk på å opprette booking med ugyldige data.");

                return View(booking);
            }

            try
            {
                _logger.LogInformation(
                    "Oppretter booking for {StudentName} i {RoomName}.",
                    booking.StudentName,
                    booking.RoomName);

                _context.Add(booking);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Booking opprettet med ID {BookingId}.",
                    booking.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Feil ved oppretting av booking.");

                ModelState.AddModelError(
                    "",
                    "Det oppstod en feil ved lagring av bookingen.");

                return View(booking);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning(
                    "Edit ble kalt uten booking-ID.");

                return NotFound();
            }

            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    _logger.LogWarning(
                        "Fant ikke booking {BookingId} for redigering.",
                        id);

                    return NotFound();
                }

                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Feil ved henting av booking {BookingId}.",
                    id);

                return Problem(
                    "Det oppstod en feil ved henting av bookingen.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,StudentName,RoomName,Subject,Topic,Date,StartTime,EndTime")]
            Booking booking)
        {
            if (id != booking.Id)
            {
                _logger.LogWarning(
                    "Booking-ID stemmer ikke.");

                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Ugyldige data ved redigering av booking {BookingId}.",
                    id);

                return View(booking);
            }

            try
            {
                _context.Update(booking);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Booking {BookingId} ble oppdatert.",
                    booking.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(
                    ex,
                    "Concurrency-feil ved booking {BookingId}.",
                    booking.Id);

                if (!BookingExists(booking.Id))
                {
                    return NotFound();
                }

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Feil ved oppdatering av booking {BookingId}.",
                    booking.Id);

                ModelState.AddModelError(
                    "",
                    "Det oppstod en feil ved oppdatering av bookingen.");

                return View(booking);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning(
                    "Delete ble kalt uten booking-ID.");

                return NotFound();
            }

            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (booking == null)
                {
                    _logger.LogWarning(
                        "Fant ikke booking {BookingId} for sletting.",
                        id);

                    return NotFound();
                }

                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Feil ved henting av booking {BookingId}.",
                    id);

                return Problem(
                    "Det oppstod en feil ved henting av bookingen.");
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _logger.LogInformation(
                    "Forsøker å slette booking {BookingId}.",
                    id);

                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    _logger.LogWarning(
                        "Fant ikke booking {BookingId}.",
                        id);

                    return NotFound();
                }

                _context.Bookings.Remove(booking);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Booking {BookingId} ble slettet.",
                    id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Feil ved sletting av booking {BookingId}.",
                    id);

                return Problem(
                    "Det oppstod en feil ved sletting av bookingen.");
            }
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}