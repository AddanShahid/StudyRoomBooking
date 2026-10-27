using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudyRoomBooking.Models
{
    // A reservation of one room for a time slot on a single day.
    // Rules that involve several fields are checked in Validate();
    // overlap with other bookings needs the database and is checked in the controller.
    public class Booking : IValidatableObject
    {
        // Rooms can only be booked within opening hours, for a limited time.
        public static readonly TimeOnly OpeningTime = new(8, 0);
        public static readonly TimeOnly ClosingTime = new(22, 0);
        public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);
        public const int MaxDaysAhead = 60;

        public int Id { get; set; }

        [Display(Name = "Student name")]
        [Required(ErrorMessage = "Student name is required.")]
        [StringLength(60, MinimumLength = 2, ErrorMessage = "The name must be between 2 and 60 characters.")]
        public string StudentName { get; set; } = string.Empty;

        [Display(Name = "Room")]
        [Range(1, int.MaxValue, ErrorMessage = "Please choose a room.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(100, ErrorMessage = "The subject can be at most 100 characters.")]
        public string Subject { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "The topic can be at most 200 characters.")]
        public string? Topic { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Display(Name = "Start time")]
        [Required(ErrorMessage = "Start time is required.")]
        public TimeOnly StartTime { get; set; } = new(10, 0);

        [Display(Name = "End time")]
        [Required(ErrorMessage = "End time is required.")]
        public TimeOnly EndTime { get; set; } = new(12, 0);

        [ValidateNever]
        public virtual Room? Room { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime >= EndTime)
            {
                yield return new ValidationResult(
                    "End time must be after start time.",
                    new[] { nameof(StartTime), nameof(EndTime) }
                );
            }
            else if (EndTime - StartTime > MaxDuration)
            {
                yield return new ValidationResult(
                    $"A booking can last at most {MaxDuration.TotalHours} hours.",
                    new[] { nameof(EndTime) }
                );
            }

            if (StartTime < OpeningTime || EndTime > ClosingTime)
            {
                yield return new ValidationResult(
                    $"Rooms can only be booked between {OpeningTime:HH:mm} and {ClosingTime:HH:mm}.",
                    new[] { nameof(StartTime) }
                );
            }

            if (Date.Date.Add(StartTime.ToTimeSpan()) < DateTime.Now)
            {
                yield return new ValidationResult(
                    "You cannot book a time that has already passed.",
                    new[] { nameof(Date) }
                );
            }
            else if (Date.Date > DateTime.Today.AddDays(MaxDaysAhead))
            {
                yield return new ValidationResult(
                    $"Rooms can be booked at most {MaxDaysAhead} days ahead.",
                    new[] { nameof(Date) }
                );
            }
        }
    }
}
