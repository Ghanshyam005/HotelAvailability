using HotelAvailability.Models;
using HotelAvailability.Services;
using System;

namespace HotelAvailability
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Console App for the Hotel Booking");

            try
            {
                var (hotels, bookings) = DataLoader.LoadFromOutputDataFolder();
                Console.WriteLine($"Loaded hotels: {hotels.Count}, bookings: {bookings.Count}");
                
                // Get data from JSon file 
                AvailabilityService availabilityService = new AvailabilityService(hotels, bookings);

                Hotel h1 = hotels.First(h => h.Id == "H1");
                Console.WriteLine($"Hotel {h1.Id} - {h1.Name}:");
                Console.WriteLine($"  Rooms total: {h1.Rooms.Count}");
                Console.WriteLine($"  RoomTypes: {string.Join(", ", h1.RoomTypes.Select(rt => rt.Code))}");

                Console.WriteLine("------------------------------");
                Console.WriteLine("Try below command:");
                Console.WriteLine("  Availability(H1, 20251114, SGL)");
                Console.WriteLine("  Search(H1, 30, SGL)");
                Console.WriteLine("------------------------------");

                while (true)
                {
                    Console.Write("> ");
                    string input = Console.ReadLine();

                    // EXIT
                    if (string.IsNullOrWhiteSpace(input))
                        break;

                    string cmd = input.Trim();

                    // ----------------------------------------------------
                    // AVAILABILITY
                    // ----------------------------------------------------
                    if (cmd.StartsWith("Availability"))
                    {
                        try
                        {
                            List<string> argsList = ExtractArguments(cmd);

                            if (argsList.Count != 3)
                            {
                                Console.WriteLine("Usage: Availability(hotelId, dateOrRange, roomType)");
                                break;
                            }

                            string hotelId = argsList[0];
                            string datePart = argsList[1];
                            string roomType = argsList[2];
                            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

                            // Single date
                            if (!datePart.Contains('-'))
                            {
                                DateOnly date = DateOnly.ParseExact(datePart, "yyyyMMdd");
                                if (date < today )
                                {
                                    Console.WriteLine("Invalid input: availability cannot be checked for past dates.");
                                    break;
                                }

                                int result = availabilityService.GetAvailabilityForDate(hotelId, roomType, date);

                                Console.WriteLine($"Availability = {result}");
                                break;
                            }

                            // Date range
                            string[] range = datePart.Split('-');
                            DateOnly startDate = DateOnly.ParseExact(range[0], "yyyyMMdd");
                            DateOnly endDate = DateOnly.ParseExact(range[1], "yyyyMMdd");
                            
                            if (startDate < today || endDate < today)
                            {
                                Console.WriteLine("Invalid input: availability cannot be checked for past dates.");
                            }

                            int rangeResult = availabilityService.GetAvailabilityForRange(hotelId, roomType, startDate, endDate);
                            Console.WriteLine($"Availability = {rangeResult}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }

                        continue;
                    }

                    // ----------------------------------------------------
                    // SEARCH
                    // ----------------------------------------------------
                    if (cmd.StartsWith("Search"))
                    {
                        try
                        {
                            List<string> argsList = ExtractArguments(cmd);

                            if (argsList.Count != 3)
                            {
                                Console.WriteLine("Usage: Search(hotelId, daysAhead, roomType)");
                                break;
                            }

                            string hotelId = argsList[0];
                            int daysAhead = int.Parse(argsList[1]);
                            string roomType = argsList[2];

                            var results = availabilityService.SearchAvailability(hotelId, roomType, daysAhead);

                            if (results.Count == 0)
                            {
                                Console.WriteLine("");
                                break;
                            }

                            var formatted = results.Select(r =>
                                $"({r.Start:yyyyMMdd}-{r.End:yyyyMMdd}, {r.Availability})"
                            );

                            Console.WriteLine(string.Join(", ", formatted));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }

                        break;
                    }

                    // ----------------------------------------------------
                    // UNKNOWN COMMAND
                    // ----------------------------------------------------
                    Console.WriteLine("Unknown command. Try below command:");
                    Console.WriteLine("  Availability(H1, 20251114, SGL)");
                    Console.WriteLine("  Search(H1, 30, SGL)");
                }

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Load error: " + ex.Message);
            }

        }

        static List<string> ExtractArguments(string command)
        {
            int open = command.IndexOf('(');
            int close = command.LastIndexOf(')');

            if (open < 0 || close < 0 || close <= open)
                throw new Exception("Invalid command format");

            // Get the text inside parentheses
            string inside = command.Substring(open + 1, close - open - 1);

            // Split by commas
            return inside.Split(',', StringSplitOptions.TrimEntries).ToList();
        }

    }
}
