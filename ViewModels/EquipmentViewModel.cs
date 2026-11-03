namespace StudyRoomBooking.ViewModels
{
    public class EquipmentViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Quantity { get; set; }
    }
}