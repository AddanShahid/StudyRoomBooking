using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.DAL
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<RoomRepository> _logger;

        public RoomRepository(ApplicationDbContext db, ILogger<RoomRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Room>?> GetAll()
        {
            try
            {
                // Bookings are included so the views can show how busy each room is.
                return await _db.Rooms
                    .Include(r => r.Bookings)
                    .OrderBy(r => r.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[RoomRepository] GetAll() failed");
                return null;
            }
        }

        public async Task<Room?> GetById(int id)
        {
            try
            {
                return await _db.Rooms
                    .Include(r => r.Bookings)
                    .FirstOrDefaultAsync(r => r.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[RoomRepository] GetById() failed for RoomId {RoomId}", id);
                return null;
            }
        }

        public async Task<bool> NameExists(string name, int? exceptRoomId = null)
        {
            try
            {
                var lowered = name.Trim().ToLower();
                return await _db.Rooms.AnyAsync(r => r.Name.ToLower() == lowered && r.Id != exceptRoomId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[RoomRepository] NameExists() failed for name {Name}", name);
                return false;
            }
        }

        public async Task<bool> Create(Room room)
        {
            try
            {
                room.Name = room.Name.Trim();
                _db.Rooms.Add(room);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[RoomRepository] Create() failed for room {RoomName}", room.Name);
                return false;
            }
        }

        public async Task<bool> Update(Room room)
        {
            try
            {
                room.Name = room.Name.Trim();
                _db.Rooms.Update(room);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[RoomRepository] Update() failed for RoomId {RoomId}", room.Id);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var room = await _db.Rooms.FindAsync(id);
                if (room == null)
                {
                    _logger.LogWarning("[RoomRepository] Delete(): room {RoomId} not found", id);
                    return false;
                }

                _db.Rooms.Remove(room);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[RoomRepository] Delete() failed for RoomId {RoomId}", id);
                return false;
            }
        }
    }
}
