using HotelAvailability.Models;
using HotelAvailability.Services;
using HotelAvailability.Helper;
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
                //Read data from JSON files    
                (List<Hotel> hotels, List<Booking> bookings) = DataLoader.LoadHotelBookingData();

                AvailabilityService availabilityService = new AvailabilityService(hotels, bookings);
                Utilities utilities = new Utilities();

                while (true)
                {                   
                    string userInput = Console.ReadLine().Trim();

                    // EXIT on empty input
                    if (string.IsNullOrWhiteSpace(userInput))
                        break;

                    List<string> argsList = utilities.ExtractArguments(userInput);

                    #region Availability check 
                    if (userInput.StartsWith("Availability"))
                    {
                        utilities.CheckAvailability(availabilityService, argsList, out var message);
                        Console.WriteLine(message);
                        continue;
                    }
                    #endregion

                    #region SEARCH 
                    if (userInput.StartsWith("Search"))
                    {
                        utilities.SearchAvailability(availabilityService, argsList, out var message);
                        Console.WriteLine(message);
                        continue;
                    }
                    #endregion

                    #region Unknown command hit 
                    Console.WriteLine("Unknown command. Please use below command:");
                    Console.WriteLine("  Availability(H1, 20251127, SGL)");
                    Console.WriteLine("  Search(H1, 30, SGL)");
                    #endregion
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Load error: " + ex.Message);
            }
        }
    }
}
