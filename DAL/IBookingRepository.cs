using StudyRoomBooking.Models;

namespace StudyRoomBooking.DAL
{
    // Data access for bookings. Read methods return null and write methods return false
    // if the database operation fails; the error is logged by the repository.
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>?> GetAll();
        Task<Booking?> GetById(int id);

        // Returns the first booking of the same room that overlaps the given one, or null.
        Task<Booking?> FindOverlap(Booking booking);

        Task<bool> Create(Booking booking);
        Task<bool> Update(Booking booking);
        Task<bool> Delete(int id);
    }
}
