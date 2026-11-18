namespace SubApp2.Api.DTOs;

public class BookingDto
{
    public int Id { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int RoomId { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string? Topic { get; set; }

    public DateTime Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}