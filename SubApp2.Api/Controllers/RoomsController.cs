using StudyRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.DAL;
using SubApp2.Api.DTOs;

namespace SubApp2.Api.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;

    public RoomsController(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
    {
        var rooms = await _roomRepository.GetAll();

        if (rooms == null)
        {
            return StatusCode(500, "Could not load rooms.");
        }

        var roomDtos = rooms.Select(room => new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Location = room.Location,
            Capacity = room.Capacity,
            Description = room.Description,
            HasWhiteboard = room.HasWhiteboard,
            HasScreen = room.HasScreen,
            OpeningTime = room.OpeningTime,
            ClosingTime = room.ClosingTime
        }).ToList();

        return Ok(roomDtos);

    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoomDto>> GetRoom(int id)
    {
        var room = await _roomRepository.GetById(id);

        if (room == null)
        {
            return NotFound($"Room with ID {id} was not found.");
        }

        var roomDto = new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Location = room.Location,
            Capacity = room.Capacity,
            Description = room.Description,
            HasWhiteboard = room.HasWhiteboard,
            HasScreen = room.HasScreen,
            OpeningTime = room.OpeningTime,
            ClosingTime = room.ClosingTime
        };

        return Ok(roomDto);
    }
    [HttpPost]
    public async Task<ActionResult<RoomDto>> CreateRoom(CreateRoomDto dto)
    {
        if (dto.OpeningTime >= dto.ClosingTime)
        {
            return BadRequest("Opening time must be before closing time.");
        }

        var roomName = dto.Name.Trim();

        if (roomName.Length < 2)
        {
            return BadRequest("Room name must be at least 2 characters long.");
        }

        if (await _roomRepository.NameExists(roomName))
        {
            return Conflict("A room with this name already exists.");
        }



        var room = new Room
        {
            Name = roomName,
            Location = dto.Location,
            Capacity = dto.Capacity,
            Description = dto.Description,
            HasWhiteboard = dto.HasWhiteboard,
            HasScreen = dto.HasScreen,
            OpeningTime = dto.OpeningTime,
            ClosingTime = dto.ClosingTime
        };

        var success = await _roomRepository.Create(room);

        if (!success)
        {
            return StatusCode(500, "Could not create room.");
        }

        var roomDto = new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Location = room.Location,
            Capacity = room.Capacity,
            Description = room.Description,
            HasWhiteboard = room.HasWhiteboard,
            HasScreen = room.HasScreen,
            OpeningTime = room.OpeningTime,
            ClosingTime = room.ClosingTime
        };

        return CreatedAtAction(
            nameof(GetRoom),
            new { id = room.Id },
            roomDto
        );
    }
    [HttpPut("{id:int}")]
public async Task<IActionResult> UpdateRoom(int id, CreateRoomDto dto)
{
    var room = await _roomRepository.GetById(id);

    if (room == null)
    {
        return NotFound($"Room with ID {id} was not found.");
    }

    if (dto.OpeningTime >= dto.ClosingTime)
    {
        return BadRequest("Opening time must be before closing time.");
    }

    var roomName = dto.Name.Trim();

    if (roomName.Length < 2)
    {
        return BadRequest("Room name must be at least 2 characters long.");
    }

    if (await _roomRepository.NameExists(roomName, id))
    {
        return Conflict("A room with this name already exists.");
    }

    room.Name = roomName;
    room.Location = dto.Location;
    room.Capacity = dto.Capacity;
    room.Description = dto.Description;
    room.HasWhiteboard = dto.HasWhiteboard;
    room.HasScreen = dto.HasScreen;
    room.OpeningTime = dto.OpeningTime;
    room.ClosingTime = dto.ClosingTime;

    var success = await _roomRepository.Update(room);

    if (!success)
    {
        return StatusCode(500, "Could not update room.");
    }

    return NoContent();
}
[HttpDelete("{id:int}")]
public async Task<IActionResult> DeleteRoom(int id)
{
    var room = await _roomRepository.GetById(id);

    if (room == null)
    {
        return NotFound($"Room with ID {id} was not found.");
    }

    var success = await _roomRepository.Delete(id);

    if (!success)
    {
        return StatusCode(500, "Could not delete room.");
    }

    return NoContent();
}


}