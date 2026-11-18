using StudyRoomBooking.Models;

namespace StudyRoomBooking.ViewModels
{
    public class RoomViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Location { get; set; }

        public int Capacity { get; set; }

        public string? Description { get; set; }

        public TimeOnly OpeningTime { get; set; }

        public TimeOnly ClosingTime { get; set; }

        public List<Equipment> Equipments { get; set; } = new List<Equipment>();
    }
}