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
    
}