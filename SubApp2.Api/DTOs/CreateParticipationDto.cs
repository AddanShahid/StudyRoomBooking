using System.ComponentModel.DataAnnotations;

namespace SubApp2.Api.DTOs;

public class CreateParticipationDto
{
    [Required]
    [StringLength(60, MinimumLength = 2)]
    public string StudentName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int BookingId { get; set; }

    [StringLength(500)]
    public string? Message { get; set; }
}