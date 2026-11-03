
using System.ComponentModel.DataAnnotations;

namespace StudyRoomBooking.Models
{
    // hvilke student som deltar i en booking 
    public class Participation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Student name is required.")]
        [StringLength(60)]
        public string StudentName { get; set; } = string.Empty;

        public int BookingId { get; set; }

        public virtual Booking? Booking { get; set; }

        [StringLength(500)]
        public string? Message { get; set; }
    }
}
