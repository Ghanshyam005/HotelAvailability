using HotelAvailability.Models;

namespace HotelAvailability.Services
{
    public class AvailabilityService
    {
        private readonly List<Hotel> _hotels;
        private readonly List<Booking> _bookings;

        // Constructor receives loaded data
        public AvailabilityService(List<Hotel> hotels, List<Booking> bookings)
        {
            _hotels = hotels;
            _bookings = bookings;
        }

        // -----------------------------------
        // 1. Calculate availability for ONE date
        // -----------------------------------
        public int GetAvailabilityForDate(string hotelId, string roomType, DateOnly date)
        {
            var hotel = _hotels.FirstOrDefault(h => h.Id.Equals(hotelId, StringComparison.OrdinalIgnoreCase));

            if (hotel == null)
                throw new Exception("Hotel not found: " + hotelId);

            // Count rooms of that type
            int totalRooms = hotel.Rooms.Count(r => r.RoomType == roomType);

            // Count how many bookings overlap this date
            int booked = _bookings.Count(b =>
                b.HotelId == hotelId &&
                b.RoomType == roomType &&
                b.ArrivalDate <= date &&
                date < b.DepartureDate // (arrival inclusive, departure exclusive)
            );

            return totalRooms - booked;
        }

        // -----------------------------------
        // 2. Availability for date RANGE
        // -----------------------------------
        public int GetAvailabilityForRange(string hotelId, string roomType, DateOnly start, DateOnly end)
        {
            // For ranges we return the MIN availability across all days
            int minAvailability = int.MaxValue;

            for (var d = start; d <= end; d = d.AddDays(1))
            {
                int avail = GetAvailabilityForDate(hotelId, roomType, d);
                if (avail < minAvailability)
                    minAvailability = avail;
            }

            return minAvailability;
        }

        public List<(DateOnly Start, DateOnly End, int Availability)> SearchAvailability(string hotelId, string roomType, int daysAhead)
        {
            var result = new List<(DateOnly, DateOnly, int)>();

            // Start from TODAY
            DateOnly today = DateOnly.FromDateTime(DateTime.Now.Date);
            DateOnly endDate = today.AddDays(daysAhead);

            DateOnly? rangeStart = null;
            DateOnly? prevDate = null;
            int currentAvail = 0;

            for (var d = today; d <= endDate; d = d.AddDays(1))
            {
                int dayAvail = GetAvailabilityForDate(hotelId, roomType, d);

                if (dayAvail > 0)
                {
                    // Start of a new range
                    if (rangeStart == null)
                    {
                        rangeStart = d;
                        prevDate = d;
                        currentAvail = dayAvail;
                    }
                    else if (d == prevDate.Value.AddDays(1) && dayAvail == currentAvail)
                    {
                        // Continue the range
                        prevDate = d;
                    }
                    else
                    {
                        // End and record previous range
                        result.Add((rangeStart.Value, prevDate.Value, currentAvail));

                        // Start new range
                        rangeStart = d;
                        prevDate = d;
                        currentAvail = dayAvail;
                    }
                }
                else
                {
                    // Close active range
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

    }
}
