using System.Text.Json;
using HotelAvailability.Models;

namespace HotelAvailability.Services
{
    public static class DataLoader
    {
        public static (List<Hotel> Hotels, List<Booking> Bookings) LoadHotelBookingData()
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

            Console.WriteLine($"Loaded hotels: {hotels.Count}, bookings: {bookings.Count}");

            if (hotels.Count == 0)
            {
                Console.WriteLine("No hotels found in data.");
            }
            else
            {
                foreach (var hotel in hotels)
                {
                    var roomCount = hotel.Rooms?.Count ?? 0;
                    var roomTypeCodes = (hotel.RoomTypes != null && hotel.RoomTypes.Any())
                        ? string.Join(", ", hotel.RoomTypes.Select(rt => rt.Code))
                        : "(none)";

                    Console.WriteLine($"Hotel {hotel.Id} - {hotel.Name}:");
                    Console.WriteLine($"  Rooms total: {roomCount}");
                    Console.WriteLine($"  RoomTypes: {roomTypeCodes}");
                    Console.WriteLine();
                }
            }

            Console.WriteLine("------------------------------");
            Console.WriteLine("Try below command for availability and hotel search:");
            Console.WriteLine("  Availability(H1, 20251127, SGL)");
            Console.WriteLine("  Search(H1, 30, SGL)");
            Console.WriteLine("------------------------------");
            Console.Write("> ");

            return (hotels, bookings);
        }
    }
}
