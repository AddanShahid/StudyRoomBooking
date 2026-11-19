
using Microsoft.EntityFrameworkCore;
using StudyRoomBooking.Data;
using StudyRoomBooking.Models;

namespace StudyRoomBooking.DAL
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EquipmentRepository> _logger;

        public EquipmentRepository(
            ApplicationDbContext db,
            ILogger<EquipmentRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Equipment>?> GetAll()
        {
            try
            {
                return await _db.Equipments
                    .Include(e => e.Rooms)
                    .OrderBy(e => e.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[EquipmentRepository] GetAll() failed");
                return null;
            }
        }

        public async Task<Equipment?> GetById(int id)
        {
            try
            {
                return await _db.Equipments
                    .Include(e => e.Rooms)
                    .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[EquipmentRepository] GetById() failed for EquipmentId {EquipmentId}", id);
                return null;
            }
        }

        public async Task<bool> Create(Equipment equipment)
        {
            try
            {
                equipment.Name = equipment.Name.Trim();

                _db.Equipments.Add(equipment);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[EquipmentRepository] Create() failed");
                return false;
            }
        }

        public async Task<bool> Update(int id, Equipment equipment)
        {
            try
            {
                var existingEquipment = await _db.Equipments.FindAsync(id);

                if (existingEquipment == null)
                {
                    _logger.LogWarning("[EquipmentRepository] Update(): equipment {EquipmentId} not found", id);
                    return false;
                }

                existingEquipment.Name = equipment.Name.Trim();
                existingEquipment.Description = equipment.Description;

                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[EquipmentRepository] Update() failed for EquipmentId {EquipmentId}", id);
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var equipment = await _db.Equipments.FindAsync(id);

                if (equipment == null)
                {
                    _logger.LogWarning("[EquipmentRepository] Delete(): equipment {EquipmentId} not found", id);
                    return false;
                }

                _db.Equipments.Remove(equipment);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[EquipmentRepository] Delete() failed for EquipmentId {EquipmentId}", id);
                return false;
            }
        }
    }
}
