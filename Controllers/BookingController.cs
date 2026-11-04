
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudyRoomBooking.DAL;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            IBookingRepository bookingRepository,
            IRoomRepository roomRepository,
            ILogger<BookingController> logger)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _logger = logger;
        }

        // Show bookings and apply search filters
        public async Task<IActionResult> Index(
            string? subject,
            DateTime? date,
            int? roomId,
            int? equipmentId)
        {
            _logger.LogInformation("Henter bookinger.");

            var bookings = await _bookingRepository.GetAll();

            if (bookings == null)
            {
                return Problem("Kunne ikke hente bookinger.");
            }

            var rooms = await _roomRepository.GetAll();

            if (rooms == null)
            {
                return Problem("Kunne ikke hente rom.");
            }

            ViewBag.Rooms = rooms.ToList();

            ViewBag.Equipments = rooms
                .SelectMany(r => r.Equipment)
                .GroupBy(e => e.Id)
                .Select(g => g.First())
                .OrderBy(e => e.Name)
                .ToList();

            ViewBag.SelectedSubject = subject;
            ViewBag.SelectedDate = date;
            ViewBag.SelectedRoomId = roomId;
            ViewBag.SelectedEquipmentId = equipmentId;

            if (!string.IsNullOrWhiteSpace(subject))
            {
                bookings = bookings.Where(b =>
                    b.Subject.Contains(
                        subject.Trim(),
                        StringComparison.OrdinalIgnoreCase));
            }

            if (date.HasValue)
            {
                bookings = bookings.Where(b =>
                    b.Date.Date == date.Value.Date);
            }

            if (roomId.HasValue)
            {
                bookings = bookings.Where(b =>
                    b.RoomId == roomId.Value);
            }

            if (equipmentId.HasValue)
            {
                var matchingRoomIds = rooms
                    .Where(r => r.Equipment.Any(e =>
                        e.Id == equipmentId.Value))
                    .Select(r => r.Id)
                    .ToHashSet();

                bookings = bookings.Where(b =>
                    matchingRoomIds.Contains(b.RoomId));
            }

            return View(bookings.ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning(
                    "Details ble kalt uten booking-ID.");

                return NotFound();
            }

            var booking = await _bookingRepository.GetById(id.Value);

            if (booking == null)
            {
                _logger.LogWarning(
                    "Fant ikke booking med ID {BookingId}.", id);

                return NotFound();
            }

            return View(booking);
        }

        // Display the booking form and search for available rooms
        [Authorize]
        public async Task<IActionResult> Create(
            DateTime? date,
            TimeOnly? startTime,
            TimeOnly? endTime)
        {
            _logger.LogInformation("Create-siden ble åpnet.");

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

            if (date.HasValue &&
                startTime.HasValue &&
                endTime.HasValue &&
                startTime.Value < endTime.Value)
            {
                ViewBag.Rooms = await GetAvailableRooms(
                    date.Value,
                    startTime.Value,
                    endTime.Value);
            }

            return View(booking);
        }

        // Save a new booking
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("StudentName,RoomId,Subject,Topic,Date,StartTime,EndTime")]
            Booking booking)
        {
            await ValidateRoomAndOverlap(booking);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Forsøk på å opprette booking med ugyldige data.");

                ViewBag.SearchDate = booking.Date;
                ViewBag.SearchStartTime = booking.StartTime;
                ViewBag.SearchEndTime = booking.EndTime;

                ViewBag.Rooms = await GetAvailableRooms(
                    booking.Date,
                    booking.StartTime,
                    booking.EndTime);

                return View(booking);
            }

            if (!await _bookingRepository.Create(booking))
            {
                ModelState.AddModelError(
                    "",
                    "Det oppstod en feil ved lagring av bookingen.");

                ViewBag.SearchDate = booking.Date;
                ViewBag.SearchStartTime = booking.StartTime;
                ViewBag.SearchEndTime = booking.EndTime;

                ViewBag.Rooms = await GetAvailableRooms(
                    booking.Date,
                    booking.StartTime,
                    booking.EndTime);

                return View(booking);
            }

            _logger.LogInformation(
                "Booking opprettet med ID {BookingId} for {StudentName} i rom {RoomId}.",
                booking.Id,
                booking.StudentName,
                booking.RoomId);

            return RedirectToAction(nameof(Index));
        }

        // Display Edit form
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning(
                    "Edit ble kalt uten booking-ID.");

                return NotFound();
            }

            var booking = await _bookingRepository.GetById(id.Value);

            if (booking == null)
            {
                _logger.LogWarning(
                    "Fant ikke booking {BookingId} for redigering.", id);

                return NotFound();
            }

            await LoadRoomOptions(booking.RoomId);

            return View(booking);
        }

        // Update booking
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,StudentName,RoomId,Subject,Topic,Date,StartTime,EndTime")]
            Booking booking)
        {
            if (id != booking.Id)
            {
                _logger.LogWarning("Booking-ID stemmer ikke.");

                return NotFound();
            }

            await ValidateRoomAndOverlap(booking);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Ugyldige data ved redigering av booking {BookingId}.",
                    id);

                await LoadRoomOptions(booking.RoomId);

                return View(booking);
            }

            if (!await _bookingRepository.Update(booking))
            {
                ModelState.AddModelError(
                    "",
                    "Det oppstod en feil ved oppdatering av bookingen.");

                await LoadRoomOptions(booking.RoomId);

                return View(booking);
            }

            _logger.LogInformation(
                "Booking {BookingId} ble oppdatert.",
                booking.Id);

            return RedirectToAction(nameof(Index));
        }

        // Display Delete confirmation
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning(
                    "Delete ble kalt uten booking-ID.");

                return NotFound();
            }

            var booking = await _bookingRepository.GetById(id.Value);

            if (booking == null)
            {
                _logger.LogWarning(
                    "Fant ikke booking {BookingId} for sletting.",
                    id);

                return NotFound();
            }

            return View(booking);
        }

        // Delete booking
        [Authorize]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation(
                "Forsøker å slette booking {BookingId}.",
                id);

            if (!await _bookingRepository.Delete(id))
            {
                return Problem(
                    "Det oppstod en feil ved sletting av bookingen.");
            }

            _logger.LogInformation(
                "Booking {BookingId} ble slettet.",
                id);

            return RedirectToAction(nameof(Index));
        }

        // Validate room and overlapping bookings
        private async Task ValidateRoomAndOverlap(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            if (await _roomRepository.GetById(booking.RoomId) == null)
            {
                ModelState.AddModelError(
                    nameof(Booking.RoomId),
                    "The chosen room does not exist.");

                return;
            }

            var overlap = await _bookingRepository.FindOverlap(booking);

            if (overlap != null)
            {
                _logger.LogWarning(
                    "Rom {RoomId} er allerede booket {Date:d} {Start}-{End}.",
                    booking.RoomId,
                    booking.Date,
                    overlap.StartTime,
                    overlap.EndTime);

                ModelState.AddModelError(
                    nameof(Booking.StartTime),
                    $"The room is already booked from {overlap.StartTime:HH:mm} to {overlap.EndTime:HH:mm} on this day.");
            }
        }

        // Find available rooms
        private async Task<List<Room>> GetAvailableRooms(
            DateTime date,
            TimeOnly startTime,
            TimeOnly endTime)
        {
            var rooms = await _roomRepository.GetAll();
            var bookings = await _bookingRepository.GetAll();

            if (rooms == null || bookings == null)
            {
                _logger.LogError(
                    "Could not retrieve rooms or bookings when searching for available rooms.");

                return new List<Room>();
            }

            var bookedRoomIds = bookings
                .Where(b =>
                    b.Date.Date == date.Date &&
                    b.StartTime < endTime &&
                    startTime < b.EndTime)
                .Select(b => b.RoomId)
                .ToHashSet();

            return rooms
                .Where(room => !bookedRoomIds.Contains(room.Id))
                .ToList();
        }

        // Fill room dropdown
        private async Task LoadRoomOptions(
            int? selectedRoomId = null)
        {
            var rooms = await _roomRepository.GetAll()
                ?? Enumerable.Empty<Room>();

            ViewData["RoomId"] = new SelectList(
                rooms.Select(r => new
                {
                    r.Id,
                    Label = $"{r.Name} ({r.Location}, {r.Capacity} seats)"
                }),
                "Id",
                "Label",
                selectedRoomId);
        }
    }
}
