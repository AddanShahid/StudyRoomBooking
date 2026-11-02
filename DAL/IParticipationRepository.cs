using StudyRoomBooking.Models;

namespace StudyRoomBooking.DAL
{
    public interface IParticipationRepository
    {
        Task<IEnumerable<Participation>?> GetAll();
        Task<Participation?> GetById(int id);
        Task<IEnumerable<Participation>?> GetByBookingId(int bookingId);
        Task<bool> Create(Participation participation);
        Task<bool> Delete(int id);
    }
}