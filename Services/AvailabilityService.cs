using HotelAvailability.Models;

namespace HotelAvailability.Services
{
    public class AvailabilityService
    {
        private readonly List<Hotel> _hotels;
        private readonly List<Booking> _bookings;

        public AvailabilityService(List<Hotel> hotels, List<Booking> bookings)
        {
            _hotels = hotels;
            _bookings = bookings;
        }
        
        public int GetAvailabilityRoomCountForDate(string hotelId, string roomType, DateOnly date)
        {
            var hotel = _hotels.FirstOrDefault(h => h.Id.Equals(hotelId, StringComparison.OrdinalIgnoreCase));

            if (hotel == null)
                throw new Exception("Hotel not found: " + hotelId);

            int totalRoomsType = hotel.Rooms.Count(r => r.RoomType == roomType);

            // Count how many bookings overlap this date
            int booked = _bookings.Count(b =>
                b.HotelId == hotelId &&
                b.RoomType == roomType &&
                b.ArrivalDate <= date &&
                date <= b.DepartureDate 
            );

            return totalRoomsType - booked;
        }
        
        public int GetAvailabilityRoomCountForRangeDate(string hotelId, string roomType, DateOnly startDay, DateOnly endDay)
        {
            // For ranges we return the MIN availability across all days
            int minAvailability = int.MaxValue;

            for (DateOnly day = startDay; day <= endDay; day = day.AddDays(1))
            {
                int availability = GetAvailabilityRoomCountForDate(hotelId, roomType, day);
                if (availability < minAvailability)
                    minAvailability = availability;
            }
            return minAvailability;
        }

    }
}
