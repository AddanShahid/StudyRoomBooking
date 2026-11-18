using System.ComponentModel.DataAnnotations;

namespace SubApp2.Api.DTOs;

public class UpdateParticipationDto
{
    [StringLength(500)]
    public string? Message { get; set; }
}
