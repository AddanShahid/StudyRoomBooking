using System.ComponentModel.DataAnnotations;

namespace SubApp2.Api.DTOs;

public class CreateBookingDto
{
    [Required]
    [StringLength(60, MinimumLength = 2)]
    public string StudentName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int RoomId { get; set; }

    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Topic { get; set; }

    public DateTime Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}