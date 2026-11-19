
using StudyRoomBooking.Models;

namespace StudyRoomBooking.DAL
{
    public interface IEquipmentRepository
    {
        Task<IEnumerable<Equipment>?> GetAll();
        Task<Equipment?> GetById(int id);
        Task<bool> Create(Equipment equipment);
        Task<bool> Update(int id, Equipment equipment);
        Task<bool> Delete(int id);
    }
}
