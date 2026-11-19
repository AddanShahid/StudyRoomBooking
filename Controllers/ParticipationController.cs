
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.DAL;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.Controllers
{
    [Authorize]
    public class ParticipationController : Controller
    {
        private readonly IParticipationRepository _participationRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<ParticipationController> _logger;

        public ParticipationController(
            IParticipationRepository participationRepository,
            IBookingRepository bookingRepository,
            ILogger<ParticipationController> logger)

        {
            _participationRepository = participationRepository;
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int bookingId,
            string? message)
        {
            var booking = await _bookingRepository.GetById(bookingId);

            if (booking == null)
            {
                _logger.LogWarning("[ParticipationController] Fant ikke booking {BookingId}",
                 bookingId);
                return NotFound();
            }

            var studentName = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(studentName))
                return Forbid();

            if (studentName.Length > 60 || message?.Length > 500)
                return BadRequest("Invalid participation details.");

            // Prevent joining the same booking twice
            if (booking.Participations != null &&
                booking.Participations.Any(p =>
                    string.Equals(
                        p.StudentName,
                        studentName,
                        StringComparison.OrdinalIgnoreCase)))
            {
                return RedirectToAction(
                    "Details", "Booking", new { id = bookingId });
            }

            var participation = new Participation
            {
                BookingId = bookingId,
                StudentName = studentName,
                Message = message
            };

            if (!await _participationRepository.Create(participation))
            {
                _logger.LogError("[ParticipationController] Kunne ikke melde {User} på booking {BookingId}",
                studentName, bookingId);
                return Problem("Could not join the booking.");
            }
            _logger.LogInformation("[ParticipationController] {User} ble med i booking {BookingId}",
                studentName, bookingId);
            return RedirectToAction(
                "Details", "Booking", new { id = bookingId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var participation = await _participationRepository.GetById(id);

            if (participation == null)
                return NotFound();

            var studentName = User.Identity?.Name;

            // Users can only remove their own participation
            if (string.IsNullOrWhiteSpace(studentName) ||
                !string.Equals(
                    participation.StudentName,
                    studentName,
                    StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("[ParticipationController] {User} prøvde å fjerne en annens påmelding {Id}",
                   studentName, id);
                return Forbid();
            }

            int bookingId = participation.BookingId;

            if (!await _participationRepository.Delete(id))
            {

                _logger.LogError("[ParticipationController] Kunne ikke fjerne påmelding {Id}",
                 id);

                return Problem("Could not leave the booking.");
            }
            _logger.LogInformation("[ParticipationController] {User} forlot booking {BookingId}",
                 studentName, bookingId);
            return RedirectToAction(
                "Details", "Booking", new { id = bookingId });
        }
    }
}
