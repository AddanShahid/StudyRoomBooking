using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Equipment name is required.")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Quantity { get; set; }

        // Kobling til flere rom
        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}