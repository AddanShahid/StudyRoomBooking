using StudyRoomBooking.Models;

namespace StudyRoomBooking.DAL
{
    // Data access for rooms. Read methods return null and write methods return false
    // if the database operation fails; the error is logged by the repository.
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>?> GetAll();
        Task<Room?> GetById(int id);
        Task<bool> NameExists(string name, int? exceptRoomId = null);
        Task<bool> Create(Room room);
        Task<bool> Update(Room room);
        Task<bool> Delete(int id);
    }
}
