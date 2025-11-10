using System.Text.Json;
using HotelAvailability.Models;

namespace HotelAvailability.Services
{
    public static class DataLoader
    {
        public static (List<Hotel> Hotels, List<Booking> Bookings) LoadFromOutputDataFolder()
        {
            string JsonFilePath = Directory.GetParent(AppContext.BaseDirectory)
                    .Parent.Parent.Parent.FullName;
            
            string hotelsPath = Path.Combine(JsonFilePath, "Data", "hotels.json");
            string bookingsPath = Path.Combine(JsonFilePath, "Data", "bookings.json");

            if (!File.Exists(hotelsPath))
                throw new FileNotFoundException($"Missing file: {hotelsPath}");
            if (!File.Exists(bookingsPath))
                throw new FileNotFoundException($"Missing file: {bookingsPath}");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var hotelsJson = File.ReadAllText(hotelsPath);
            var bookingsJson = File.ReadAllText(bookingsPath);

            var hotels = JsonSerializer.Deserialize<List<Hotel>>(hotelsJson, options) ?? new();
            var bookings = JsonSerializer.Deserialize<List<Booking>>(bookingsJson, options) ?? new();

            // Validate date parsing early
            //foreach (var b in bookings)
            //{
            //    _ = b.ArrivalDate;
            //    _ = b.DepartureDate;
            //}

            return (hotels, bookings);
        }
    }
}
