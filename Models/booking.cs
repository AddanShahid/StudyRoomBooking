using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        public string RoomName { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        public string? Topic { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}