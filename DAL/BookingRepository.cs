using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.DAL
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(ApplicationDbContext db, ILogger<BookingRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Booking>?> GetAll()
        {
            try
            {
                return await _db.Bookings
                    .Include(b => b.Room)
                    .OrderBy(b => b.Date).ThenBy(b => b.StartTime)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[BookingRepository] GetAll() failed");
                return null;
            }
        }

        public async Task<Booking?> GetById(int id)
        {
            try
            {
                return await _db.Bookings
                    .Include(b => b.Room)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[BookingRepository] GetById() failed for BookingId {BookingId}", id);
                return null;
            }
        }

        public async Task<Booking?> FindOverlap(Booking booking)
        {
            // Two time slots overlap when each one starts before the other one ends.
            // The booking itself is excluded so that editing it does not collide with its old version.
            return await _db.Bookings
                .AsNoTracking()
                .Where(b => b.RoomId == booking.RoomId
                            && b.Date == booking.Date
                            && b.Id != booking.Id
                            && b.StartTime < booking.EndTime
                            && booking.StartTime < b.EndTime)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Create(Booking booking)
        {
            try
            {
                _db.Bookings.Add(booking);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[BookingRepository] Create() failed for room {RoomId} on {Date}", booking.RoomId, booking.Date);
                return false;
            }
        }

        public async Task<bool> Update(Booking booking)
        {
            try
            {
                _db.Bookings.Update(booking);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[BookingRepository] Update() failed for BookingId {BookingId}", booking.Id);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var booking = await _db.Bookings.FindAsync(id);
                if (booking == null)
                {
                    _logger.LogWarning("[BookingRepository] Delete(): booking {BookingId} not found", id);
                    return false;
                }

                _db.Bookings.Remove(booking);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[BookingRepository] Delete() failed for BookingId {BookingId}", id);
                return false;
            }
        }
    }
}
