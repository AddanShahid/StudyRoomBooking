using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.DAL
{
    public class ParticipationRepository : IParticipationRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ParticipationRepository> _logger;

        public ParticipationRepository(
            ApplicationDbContext db,
            ILogger<ParticipationRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Participation>?> GetAll()
        {
            try
            {
                return await _db.Participations
                    .Include(p => p.Booking)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ParticipationRepository] GetAll() failed");
                return null;
            }
        }

        public async Task<Participation?> GetById(int id)
        {
            try
            {
                return await _db.Participations
                    .Include(p => p.Booking)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ParticipationRepository] GetById() failed for ParticipationId {ParticipationId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<Participation>?> GetByBookingId(int bookingId)
        {
            try
            {
                return await _db.Participations
                    .Where(p => p.BookingId == bookingId)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ParticipationRepository] GetByBookingId() failed for BookingId {BookingId}", bookingId);
                return null;
            }
        }

        public async Task<bool> Create(Participation participation)
        {
            try
            {
                _db.Participations.Add(participation);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ParticipationRepository] Create() failed");
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var participation = await _db.Participations.FindAsync(id);

                if (participation == null)
                {
                    _logger.LogWarning("[ParticipationRepository] Delete(): participation {ParticipationId} not found", id);
                    return false;
                }

                _db.Participations.Remove(participation);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ParticipationRepository] Delete() failed for ParticipationId {ParticipationId}", id);
                return false;
            }
        }
    }
}