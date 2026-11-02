using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Room name is required.")]
        public string Name { get; set; } = string.Empty;

        public string? Location { get; set; }

        public int Capacity { get; set; }

        public string? Description { get; set; }

        public TimeOnly OpeningTime { get; set; } = new TimeOnly(8, 0);

        public TimeOnly ClosingTime { get; set; } = new TimeOnly(16, 0);

        public List<Equipment> Equipments { get; set; } = new List<Equipment>();
    }
}