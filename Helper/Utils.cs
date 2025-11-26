using System;
using System.Collections.Generic;
using System.Linq;
using HotelAvailability.Services;

namespace HotelAvailability.Helper
{
    public class Utilities
    {
        public List<string> ExtractArguments(string command)
        {
            int open = command.IndexOf('(');
            int close = command.LastIndexOf(')');

            if (open < 0 || close < 0 || close <= open)
                throw new Exception("Invalid command format");

            string inside = command.Substring(open + 1, close - open - 1);
            return inside.Split(',', StringSplitOptions.TrimEntries).ToList();
        }

        public List<(DateOnly Start, DateOnly End, int Availability)> CheckRoomAvailability(AvailabilityService service, string hotelId, string roomType, int daysAhead)
        {
            var result = new List<(DateOnly, DateOnly, int)>();

            // Start from TODAY
            DateOnly today = DateOnly.FromDateTime(DateTime.Now.Date);
            DateOnly endDate = today.AddDays(daysAhead);

            DateOnly? rangeStart = null;
            DateOnly? prevDate = null;
            int currentAvail = 0;

            for (DateOnly day = today; day <= endDate; day = day.AddDays(1))
            {
                int dayAvail = service.GetAvailabilityRoomCountForDate(hotelId, roomType, day);

                if (dayAvail > 0)
                {                    
                    if (rangeStart == null)
                    {
                        rangeStart = day;
                        prevDate = day;
                        currentAvail = dayAvail;
                    }
                    else if (day == prevDate.Value.AddDays(1) && dayAvail == currentAvail)
                    {   
                        prevDate = day;
                    }
                    else
                    {   
                        result.Add((rangeStart.Value, prevDate.Value, currentAvail));
                        rangeStart = day;
                        prevDate = day;
                        currentAvail = dayAvail;
                    }
                }
                else
                {   
                    if (rangeStart != null)
                    {
                        result.Add((rangeStart.Value, prevDate.Value, currentAvail));
                        rangeStart = null;
                        prevDate = null;
                    }
                }
            }

            // If still in a range at end
            if (rangeStart != null)
            {
                result.Add((rangeStart.Value, prevDate.Value, currentAvail));
            }

            return result;
        }

        public bool CheckAvailability(AvailabilityService service, List<string> args, out string message)
        {
            message = string.Empty;
            
            if (args == null || args.Count != 3)
            {
                message = "Usage: Availability(hotelId, dateOrRange, roomType)";
                return false;
            }

            string hotelId = args[0];
            string datePart = args[1];
            string roomType = args[2];
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            try
            {
                if (!datePart.Contains('-'))
                {
                    if (!DateOnly.TryParseExact(datePart, "yyyyMMdd", out DateOnly date))
                    {
                        message = "Invalid date format. Expected yyyyMMdd.";
                        return false;
                    }   

                    if (date < today)
                    {
                        message = "Invalid input: availability cannot be checked for past dates.";
                        return false;
                    }

                    int result = service.GetAvailabilityRoomCountForDate(hotelId, roomType, date);
                    message = $"Availability = {result}";
                    return true;
                }

                string[] range = datePart.Split('-');
                DateOnly startDate = DateOnly.ParseExact(range[0], "yyyyMMdd");
                DateOnly endDate = DateOnly.ParseExact(range[1], "yyyyMMdd");

                if (startDate < today || endDate < today)
                {
                    message = "Invalid input: availability cannot be checked for past dates.";
                    return false;
                }

                int rangeResult = service.GetAvailabilityRoomCountForRangeDate(hotelId, roomType, startDate, endDate);
                message = $"Availability = {rangeResult}";
                return true;
            }
            catch (Exception ex)
            {
                message = "Error computing availability: " + ex.Message;
                return false;
            }
        }

        public bool SearchAvailability(AvailabilityService service, List<string> args, out string message)
        {
            message = string.Empty;
            if (args == null || args.Count != 3)
            {
                message = "Usage: Search(hotelId, daysAhead, roomType)";
                return false;
            }

            string hotelId = args[0];
            if (!int.TryParse(args[1], out int daysAhead))
            {
                message = "Invalid daysAhead. Please provide a non-negative integer.";
                return false;
            }

            if (daysAhead < 0)
            {
                message = "daysAhead must be zero or positive.";
                return false;
            }

            string roomType = args[2];

            try
            {
                var results = CheckRoomAvailability(service, hotelId, roomType, daysAhead);

                if (results == null || results.Count == 0)
                {                    
                    message = string.Empty;
                    return true;
                }

                var formatted = results.Select(r =>
                    $"({r.Start:yyyyMMdd}-{r.End:yyyyMMdd}, {r.Availability})"
                );

                message = string.Join(", ", formatted);
                return true;
            }
            catch (Exception ex)
            {
                message = "Error computing search: " + ex.Message;
                return false;
            }
        }
    }
}
