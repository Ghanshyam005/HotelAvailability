namespace HotelAvailability.Models
{
    public class RoomType
    {
        public string Code { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> Amenities { get; set; } = new();
        public List<string> Features { get; set; } = new();
    }
}