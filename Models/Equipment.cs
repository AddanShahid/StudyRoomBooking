
using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models
{
    // utstyr som kan være tilgjengelig i flere rom
    public class Equipment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Equipment name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }

        public virtual List<Room> Rooms { get; set; } = new();
    }
}
