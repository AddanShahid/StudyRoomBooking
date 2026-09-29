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


        //Display all the bookings from the database
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

                //Handle unexpected errors when reading from the database
                _logger.LogError(
                    ex,
                    "Feil ved henting av bookinger.");

                return Problem(
                    "Det oppstod en feil ved henting av bookingene.");
            }
        }

        //Display the details of a specific booking
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

                //Handle unexpected errors when retrieving the booking
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


        //Display the form for creating a new booking 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,StudentName,RoomName,Subject,Topic,Date,StartTime,EndTime")]
            Booking booking)
        {

            //Validate user input on the server before saving to the database
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


                //Add the new booking to the database
                _context.Add(booking);


                //Save the changes to the database
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Booking opprettet med ID {BookingId}.",
                    booking.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                //Handle unexpected database errors
                _logger.LogError(
                    ex,
                    "Feil ved oppretting av booking.");

                ModelState.AddModelError(
                    "",
                    "Det oppstod en feil ved lagring av bookingen.");

                return View(booking);
            }
        }

        //Display the form for editing an existing booking
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

                //Handle unexpected errors when retrieving the booking 
                _logger.LogError(
                    ex,
                    "Feil ved henting av booking {BookingId}.",
                    id);

                return Problem(
                    "Det oppstod en feil ved henting av bookingen.");
            }
        }

        //Update the booking after validating the submitted form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,StudentName,RoomName,Subject,Topic,Date,StartTime,EndTime")]
            Booking booking)
        {

            //Check if the booking ID in the URL matches the booking ID in the form data
            if (id != booking.Id)
            {
                _logger.LogWarning(
                    "Booking-ID stemmer ikke.");

                return NotFound();
            }

//Validate user input on the server before updating the booking in the database
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Ugyldige data ved redigering av booking {BookingId}.",
                    id);

                return View(booking);
            }

            try
            {
                //Mark the booking entity as modified 
                _context.Update(booking);

                
                // Save updated booking to the database
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Booking {BookingId} ble oppdatert.",
                    booking.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {

                //Handle conflicts when the booking has been modified or deleted by another reequest
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

                //Handle unexpected errors and log them
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

//Display the delete confirmation page for a booking
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

                //Handle the error and log it
                _logger.LogError(
                    ex,
                    "Feil ved henting av booking {BookingId}.",
                    id);

                return Problem(
                    "Det oppstod en feil ved henting av bookingen.");
            }
        }


//DeleteConfirmed action method to handle the deletion of a booking
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

// Remove the booking from the database
                _context.Bookings.Remove(booking);


// Save changes to the database
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Booking {BookingId} ble slettet.",
                    id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the error and return a problem response
                _logger.LogError(
                    ex,
                    "Feil ved sletting av booking {BookingId}.",
                    id);

                return Problem(
                    "Det oppstod en feil ved sletting av bookingen.");
            }
        }

// Private helper method to check if a booking exists        
        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}