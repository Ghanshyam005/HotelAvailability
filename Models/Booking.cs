using System.Text.Json.Serialization;

namespace HotelAvailability.Models
{
    public class Booking
    {
        public string HotelId { get; set; } = "";
        public string RoomId { get; set; } = "";
        public string Arrival { get; set; } = "";   // yyyyMMdd
        public string Departure { get; set; } = ""; // yyyyMMdd
        public string RoomType { get; set; } = "";
        public string RoomRate { get; set; } = "";

        [JsonIgnore] public DateOnly ArrivalDate => DateOnly.ParseExact(Arrival, "yyyyMMdd");
        [JsonIgnore] public DateOnly DepartureDate => DateOnly.ParseExact(Departure, "yyyyMMdd");
    }
}
