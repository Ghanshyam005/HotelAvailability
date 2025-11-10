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

                var h1 = hotels.First(h => h.Id == "H1");
                Console.WriteLine($"Hotel {h1.Id} - {h1.Name}:");
                Console.WriteLine($"  Rooms total: {h1.Rooms.Count}");
                Console.WriteLine($"  RoomTypes: {string.Join(", ", h1.RoomTypes.Select(rt => rt.Code))}");

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
                            var argsList = ExtractArguments(cmd);

                            if (argsList.Count != 3)
                            {
                                Console.WriteLine("Usage: Availability(hotelId, dateOrRange, roomType)");
                                continue;
                            }

                            string hotelId = argsList[0];
                            string datePart = argsList[1];
                            string roomType = argsList[2];

                            // Single date
                            if (!datePart.Contains('-'))
                            {
                                var date = DateOnly.ParseExact(datePart, "yyyyMMdd");
                                int result = availabilityService.GetAvailabilityForDate(hotelId, roomType, date);

                                Console.WriteLine($"Availability = {result}");
                                continue;
                            }

                            // Date range
                            var range = datePart.Split('-');
                            var start = DateOnly.ParseExact(range[0], "yyyyMMdd");
                            var end = DateOnly.ParseExact(range[1], "yyyyMMdd");

                            int rangeResult = availabilityService.GetAvailabilityForRange(hotelId, roomType, start, end);
                            Console.WriteLine($"Availability (min over range) = {rangeResult}");
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
                            var argsList = ExtractArguments(cmd);

                            if (argsList.Count != 3)
                            {
                                Console.WriteLine("Usage: Search(hotelId, daysAhead, roomType)");
                                continue;
                            }

                            string hotelId = argsList[0];
                            int daysAhead = int.Parse(argsList[1]);
                            string roomType = argsList[2];

                            var results = availabilityService.SearchAvailability(hotelId, roomType, daysAhead);

                            if (results.Count == 0)
                            {
                                Console.WriteLine("");
                                continue;
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

                        continue;
                    }

                    // ----------------------------------------------------
                    // UNKNOWN COMMAND
                    // ----------------------------------------------------
                    Console.WriteLine("Unknown command. Try below command:");
                    Console.WriteLine("  Availability(H1, 20240901, SGL)");
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
