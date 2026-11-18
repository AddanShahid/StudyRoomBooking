
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

        public ParticipationController(
            IParticipationRepository participationRepository,
            IBookingRepository bookingRepository)
        {
            _participationRepository = participationRepository;
            _bookingRepository = bookingRepository;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int bookingId,
            string? message)
        {
            var booking = await _bookingRepository.GetById(bookingId);

            if (booking == null)
                return NotFound();

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
                return Problem("Could not join the booking.");

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
                return Forbid();
            }

            int bookingId = participation.BookingId;

            if (!await _participationRepository.Delete(id))
                return Problem("Could not leave the booking.");

            return RedirectToAction(
                "Details", "Booking", new { id = bookingId });
        }
    }
}
