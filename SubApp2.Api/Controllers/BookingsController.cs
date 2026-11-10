using System.ComponentModel.DataAnnotations;
using StudyRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.DAL;
using SubApp2.Api.DTOs;

namespace SubApp2.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;

    public BookingsController(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
    {
        var bookings = await _bookingRepository.GetAll();

        if (bookings == null)
        {
            return StatusCode(500, "Could not load bookings.");
        }

        var bookingDtos = bookings.Select(booking => new BookingDto
        {
            Id = booking.Id,
            StudentName = booking.StudentName,
            RoomId = booking.RoomId,
            Subject = booking.Subject,
            Topic = booking.Topic,
            Date = booking.Date,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime
        }).ToList();

        return Ok(bookingDtos);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingDto>> GetBooking(int id)
    {
        var booking = await _bookingRepository.GetById(id);

        if (booking == null)
        {
            return NotFound($"Booking with ID {id} was not found.");
        }

        var bookingDto = new BookingDto
        {
            Id = booking.Id,
            StudentName = booking.StudentName,
            RoomId = booking.RoomId,
            Subject = booking.Subject,
            Topic = booking.Topic,
            Date = booking.Date,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime
        };

        return Ok(bookingDto);
    }
    [HttpPost]
public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto dto)
{
    var booking = new Booking
    {
        StudentName = dto.StudentName.Trim(),
        RoomId = dto.RoomId,
        Subject = dto.Subject.Trim(),
        Topic = dto.Topic,
        Date = dto.Date.Date,
        StartTime = dto.StartTime,
        EndTime = dto.EndTime
    };

    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(booking);

    if (!Validator.TryValidateObject(
        booking, validationContext, validationResults, true))
    {
        return BadRequest(validationResults.Select(r => r.ErrorMessage));
    }

    var room = await _roomRepository.GetById(booking.RoomId);

    if (room == null)
    {
        return BadRequest("The selected room does not exist.");
    }

    var overlap = await _bookingRepository.FindOverlap(booking);

    if (overlap != null)
    {
        return Conflict("The room is already booked at this time.");
    }

    var success = await _bookingRepository.Create(booking);

    if (!success)
    {
        return StatusCode(500, "Could not create booking.");
    }

    var bookingDto = new BookingDto
    {
        Id = booking.Id,
        StudentName = booking.StudentName,
        RoomId = booking.RoomId,
        Subject = booking.Subject,
        Topic = booking.Topic,
        Date = booking.Date,
        StartTime = booking.StartTime,
        EndTime = booking.EndTime
    };

    return CreatedAtAction(
        nameof(GetBooking),
        new { id = booking.Id },
        bookingDto
    );
}

[HttpPut("{id:int}")]
public async Task<IActionResult> UpdateBooking(
    int id, CreateBookingDto dto)
{
    var booking = await _bookingRepository.GetById(id);

    if (booking == null)
    {
        return NotFound($"Booking with ID {id} was not found.");
    }

    booking.StudentName = dto.StudentName.Trim();
    booking.RoomId = dto.RoomId;
    booking.Subject = dto.Subject.Trim();
    booking.Topic = dto.Topic;
    booking.Date = dto.Date.Date;
    booking.StartTime = dto.StartTime;
    booking.EndTime = dto.EndTime;

    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(booking);

    if (!Validator.TryValidateObject(
        booking, validationContext, validationResults, true))
    {
        return BadRequest(validationResults.Select(r => r.ErrorMessage));
    }

    var room = await _roomRepository.GetById(booking.RoomId);

    if (room == null)
    {
        return BadRequest("The selected room does not exist.");
    }

    var overlap = await _bookingRepository.FindOverlap(booking);

    if (overlap != null)
    {
        return Conflict("The room is already booked at this time.");
    }

    var success = await _bookingRepository.Update(booking);

    if (!success)
    {
        return StatusCode(500, "Could not update booking.");
    }

    return NoContent();
}

[HttpDelete("{id:int}")]
public async Task<IActionResult> DeleteBooking(int id)
{
    var booking = await _bookingRepository.GetById(id);

    if (booking == null)
    {
        return NotFound($"Booking with ID {id} was not found.");
    }

    var success = await _bookingRepository.Delete(id);

    if (!success)
    {
        return StatusCode(500, "Could not delete booking.");
    }

    return NoContent();
}

}