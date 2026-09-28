using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models
{
    public class Booking : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Student name is required.")]
        public string StudentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Room name is required.")]
        public string RoomName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required.")]
        public string Subject { get; set; } = string.Empty;

        public string? Topic { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public TimeOnly EndTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime >= EndTime)
            {
                yield return new ValidationResult(
                    "End time must be after start time.",
                    new[] { nameof(StartTime), nameof(EndTime) }
                );
            }
        }
    }
}