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


        // Display all the bookings from the database
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Henter alle bookinger.");

            var bookings = await _bookingRepository.GetAll();

            // The repository returns null when the database could not be read
            if (bookings == null)
            {
                return Problem("Det oppstod en feil ved henting av bookingene.");
            }

            _logger.LogInformation("Hentet {Count} bookinger.", bookings.Count());

            return View(bookings);
        }

        // Display the details of a specific booking
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details ble kalt uten booking-ID.");
                return NotFound();
            }

            var booking = await _bookingRepository.GetById(id.Value);

            if (booking == null)
            {
                _logger.LogWarning("Fant ikke booking med ID {BookingId}.", id);
                return NotFound();
            }

            return View(booking);
        }

        // Display the form for creating a new booking
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Create-siden ble åpnet.");

            await LoadRoomOptions();

            return View(new Booking());
        }


        // Save a new booking after validating the submitted form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("StudentName,RoomId,Subject,Topic,Date,StartTime,EndTime")]
            Booking booking)
        {
            // Validate user input and check that the room is free before saving
            await ValidateRoomAndOverlap(booking);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Forsøk på å opprette booking med ugyldige data.");

                await LoadRoomOptions(booking.RoomId);
                return View(booking);
            }

            if (!await _bookingRepository.Create(booking))
            {
                ModelState.AddModelError("", "Det oppstod en feil ved lagring av bookingen.");

                await LoadRoomOptions(booking.RoomId);
                return View(booking);
            }

            _logger.LogInformation(
                "Booking opprettet med ID {BookingId} for {StudentName} i rom {RoomId}.",
                booking.Id,
                booking.StudentName,
                booking.RoomId);

            return RedirectToAction(nameof(Index));
        }

        // Display the form for editing an existing booking
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit ble kalt uten booking-ID.");
                return NotFound();
            }

            var booking = await _bookingRepository.GetById(id.Value);

            if (booking == null)
            {
                _logger.LogWarning("Fant ikke booking {BookingId} for redigering.", id);
                return NotFound();
            }

            await LoadRoomOptions(booking.RoomId);
            return View(booking);
        }

        // Update the booking after validating the submitted form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,StudentName,RoomId,Subject,Topic,Date,StartTime,EndTime")]
            Booking booking)
        {
            // Check if the booking ID in the URL matches the booking ID in the form data
            if (id != booking.Id)
            {
                _logger.LogWarning("Booking-ID stemmer ikke.");
                return NotFound();
            }

            await ValidateRoomAndOverlap(booking);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Ugyldige data ved redigering av booking {BookingId}.", id);

                await LoadRoomOptions(booking.RoomId);
                return View(booking);
            }

            if (!await _bookingRepository.Update(booking))
            {
                ModelState.AddModelError("", "Det oppstod en feil ved oppdatering av bookingen.");

                await LoadRoomOptions(booking.RoomId);
                return View(booking);
            }

            _logger.LogInformation("Booking {BookingId} ble oppdatert.", booking.Id);

            return RedirectToAction(nameof(Index));
        }

        // Display the delete confirmation page for a booking
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete ble kalt uten booking-ID.");
                return NotFound();
            }

            var booking = await _bookingRepository.GetById(id.Value);

            if (booking == null)
            {
                _logger.LogWarning("Fant ikke booking {BookingId} for sletting.", id);
                return NotFound();
            }

            return View(booking);
        }


        // DeleteConfirmed action method to handle the deletion of a booking
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Forsøker å slette booking {BookingId}.", id);

            if (!await _bookingRepository.Delete(id))
            {
                return Problem("Det oppstod en feil ved sletting av bookingen.");
            }

            _logger.LogInformation("Booking {BookingId} ble slettet.", id);

            return RedirectToAction(nameof(Index));
        }

        // Domain rule: the chosen room must exist, and a room cannot be booked twice at the same time
        private async Task ValidateRoomAndOverlap(Booking booking)
        {
            // Only check the database when the form values themselves are valid
            if (!ModelState.IsValid)
            {
                return;
            }

            if (await _roomRepository.GetById(booking.RoomId) == null)
            {
                ModelState.AddModelError(nameof(Booking.RoomId), "The chosen room does not exist.");
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

        // Fill the room dropdown used by the Create and Edit forms
        private async Task LoadRoomOptions(int? selectedRoomId = null)
        {
            var rooms = await _roomRepository.GetAll() ?? Enumerable.Empty<Room>();

            ViewData["RoomId"] = new SelectList(
                rooms.Select(r => new { r.Id, Label = $"{r.Name} ({r.Location}, {r.Capacity} seats)" }),
                "Id",
                "Label",
                selectedRoomId);
        }
    }
}
