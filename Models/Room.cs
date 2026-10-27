using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StudyRoomBooking.Models
{
    // A study room that students can book.
    public class Room
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The room needs a name.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The name must be between 2 and 50 characters.")]
        [RegularExpression(@"^[\p{L}0-9 .\-]+$", ErrorMessage = "The name may only contain letters, numbers, spaces, dots and hyphens.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter where the room is located.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The location must be between 2 and 100 characters.")]
        public string Location { get; set; } = string.Empty;

        [Range(1, 50, ErrorMessage = "Capacity must be between 1 and 50 people.")]
        public int Capacity { get; set; } = 4;

        [Display(Name = "Whiteboard")]
        public bool HasWhiteboard { get; set; }

        [Display(Name = "Screen / projector")]
        public bool HasScreen { get; set; }

        [StringLength(500, ErrorMessage = "The description can be at most 500 characters.")]
        public string? Description { get; set; }

        // Navigation property filled by EF Core. Not posted from forms, so it is excluded from validation.
        [ValidateNever]
        public virtual List<Booking> Bookings { get; set; } = new();
    }
}
