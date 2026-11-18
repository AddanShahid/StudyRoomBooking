using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using StudyRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.DAL;
using SubApp2.Api.DTOs;

namespace SubApp2.Api.Controllers;

[ApiController]
[Route("api/participations")]
public class ParticipationsController : ControllerBase
{
    private readonly IParticipationRepository _participationRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<ParticipationsController> _logger;

    public ParticipationsController(
        IParticipationRepository participationRepository,
        IBookingRepository bookingRepository,
        ILogger<ParticipationsController> logger)
    {
        _participationRepository = participationRepository;
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ParticipationDto>>> GetParticipations()
    {
        var participations = await _participationRepository.GetAll();

        if (participations == null)
        {
            return StatusCode(500, "Could not load participations.");
        }

        var participationDtos = participations.Select(p => new ParticipationDto
        {
            Id = p.Id,
            StudentName = p.StudentName,
            BookingId = p.BookingId,
            Message = p.Message
        }).ToList();

        return Ok(participationDtos);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ParticipationDto>> GetParticipation(int id)
    {
        var participation = await _participationRepository.GetById(id);

        if (participation == null)
        {
            return NotFound($"Participation with ID {id} was not found.");
        }

        var participationDto = new ParticipationDto
        {
            Id = participation.Id,
            StudentName = participation.StudentName,
            BookingId = participation.BookingId,
            Message = participation.Message
        };

        return Ok(participationDto);
    }
    [HttpGet("booking/{bookingId:int}")]
    public async Task<ActionResult<IEnumerable<ParticipationDto>>> GetByBookingId(int bookingId)
    {
        var participations = await _participationRepository.GetByBookingId(bookingId);

        if (participations == null)
        {
            return StatusCode(500, "Could not load participations.");
        }

        var participationDtos = participations.Select(p => new ParticipationDto
        {
            Id = p.Id,
            StudentName = p.StudentName,
            BookingId = p.BookingId,
            Message = p.Message
        }).ToList();

        return Ok(participationDtos);
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ParticipationDto>> CreateParticipation(
    CreateParticipationDto dto)
    {
        var booking = await _bookingRepository.GetById(dto.BookingId);

        if (booking == null)
        {
            return BadRequest("The selected booking does not exist.");
        }


        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email) || email.Length > 60)
            return Forbid();

        var participation = new Participation
        {
            StudentName = email,
            BookingId = dto.BookingId,
            Message = dto.Message?.Trim()
        };

        if (participation.StudentName.Length < 2)
        {
            return BadRequest("Student name must contain at least 2 characters.");
        }
        if (string.Equals(
    booking.StudentName?.Trim(),
    participation.StudentName,
    StringComparison.OrdinalIgnoreCase))
        {
            return Conflict("The booking organizer is already registered.");
        }

        var existingParticipations =
            await _participationRepository.GetByBookingId(dto.BookingId);

        if (existingParticipations == null)
        {
            return StatusCode(500, "Could not check existing participations.");
        }

        if (existingParticipations.Any(p =>
            string.Equals(
                p.StudentName.Trim(),
                participation.StudentName,
                StringComparison.OrdinalIgnoreCase)))
        {
            return Conflict("This student has already joined this booking.");
        }
        if (booking.Room == null)
        {
            return StatusCode(500, "Could not load the room for this booking.");
        }

        if (existingParticipations.Count() + 1 >= booking.Room.Capacity)
        {
            return Conflict("The room is already at full capacity.");
        }
        var success = await _participationRepository.Create(participation);


        if (!success)
        {
            return StatusCode(500, "Could not create participation.");
        }

        _logger.LogInformation(
            "Participation {ParticipationId} created for booking {BookingId}",
            participation.Id,
            participation.BookingId);

        var participationDto = new ParticipationDto
        {
            Id = participation.Id,
            StudentName = participation.StudentName,
            BookingId = participation.BookingId,
            Message = participation.Message
        };

        return CreatedAtAction(
            nameof(GetParticipation),
            new { id = participation.Id },
            participationDto
        );
    }
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateParticipation(
        int id, UpdateParticipationDto dto)
    {
        var participation = await _participationRepository.GetById(id);

        if (participation == null)
            return NotFound();

        var email = User.FindFirstValue(ClaimTypes.Email);

        if (!User.IsInRole("Admin") &&
            (string.IsNullOrWhiteSpace(email) ||
             !string.Equals(participation.StudentName, email,
                 StringComparison.OrdinalIgnoreCase)))
            return Forbid();

        var success = await _participationRepository.Update(
            id, dto.Message?.Trim());

        if (!success)
            return StatusCode(500, "Could not update participation.");

        _logger.LogInformation("Participation {Id} updated", id);

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteParticipation(int id)
    {
        var participation = await _participationRepository.GetById(id);

        if (participation == null)
        {
            return NotFound($"Participation with ID {id} was not found.");
        }


        if (!User.IsInRole("Admin") &&
            !string.Equals(
                participation.StudentName,
                User.FindFirstValue(ClaimTypes.Email),
                StringComparison.OrdinalIgnoreCase))
            return Forbid();

        var success = await _participationRepository.Delete(id);

        if (!success)
        {
            return StatusCode(500, "Could not delete participation.");
        }

        _logger.LogInformation(
            "Participation {ParticipationId} deleted",
            id);

        return NoContent();
    }
}