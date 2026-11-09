using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudyRoomBooking.Models
{
    // A study room that students can book.
    public class Room
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The room needs a name.")]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[\p{L}0-9 .\-]+$")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter where the room is located.")]
        [StringLength(100, MinimumLength = 2)]
        public string Location { get; set; } = string.Empty;

        [Range(1, 50)]
        public int Capacity { get; set; } = 4;

        [Display(Name = "Whiteboard")]
        public bool HasWhiteboard { get; set; }

        [Display(Name = "Screen / projector")]
        public bool HasScreen { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public TimeOnly OpeningTime { get; set; } = new TimeOnly(8, 0);

        public TimeOnly ClosingTime { get; set; } = new TimeOnly(16, 0);

        [ValidateNever]
        public virtual List<Booking> Bookings { get; set; } = new();

        [ValidateNever]
        public virtual List<Equipment> Equipment { get; set; } = new();
    }
}