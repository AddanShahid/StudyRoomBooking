namespace SubApp2.Api.DTOs;

public class ParticipationDto
{
    public int Id { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int BookingId { get; set; }

    public string? Message { get; set; }
}