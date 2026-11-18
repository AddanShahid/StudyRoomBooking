using System.ComponentModel.DataAnnotations;

namespace SubApp2.Api.DTOs;

public class CreateRoomDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    [RegularExpression(@"^[\p{L}0-9 .\-]+$")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Location { get; set; } = string.Empty;

    [Range(1, 50)]
    public int Capacity { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public bool HasWhiteboard { get; set; }

    public bool HasScreen { get; set; }

    public TimeOnly OpeningTime { get; set; } = new(8, 0);

    public TimeOnly ClosingTime { get; set; } = new(16, 0);
}