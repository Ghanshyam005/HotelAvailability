**Hotel Availability Checker – Console Application**

A simple C# console application to check hotel room availability and search future availability based on hotel and booking data.

**Functionality**

This application reads hotel data and booking data from JSON files and allows the user to:
1. Check room availability for a specific date or date range
2. Search room availability for the next N days
3. Handle overbookings
4. Validate inputs (including past-date validation)
5. Exit the program by entering a blank line

**How to Run the Program**

dotnet build

**Run the application**

dotnet run -- --hotels hotels.json --bookings bookings.json

**Commands Supported**
1. Check availability for a hotel, date, or date range.
    Availability(H1, 20251110, SGL)
    Availability(H1, 20251112-20251116, SGL)

2. Search Command
    Search(H1, 35, SGL)
    Search(H1, 10, SGL)

**Exit the Program**

(blank) Enter 

Program close.
   
**Validations**

1. Past dates are not allowed (If a user enters a date earlier than today)
2. nvalid hotel ID (If hotel does not exist)
3. Invalid room type (If room type not found in hotel)
4. Incorrect date formats (Shows a validation error.)

**Example JSON Format**

1. hotels.json
[
  {
    "id": "H1",
    "name": "Hotel California",
    "roomTypes": [
      {
        "code": "SGL",
        "description": "Single Room",
        "amenities": [ "WiFi", "TV" ],
        "features": [ "Non-smoking" ]
      },
      {
        "code": "DBL",
        "description": "Double Room",
        "amenities": [ "WiFi", "TV", "Minibar" ],
        "features": [ "Non-smoking", "Sea View" ]
      }
    ],
    "rooms": [
      {
        "roomType": "SGL",
        "roomId": "101"
      },
      {
        "roomType": "SGL",
        "roomId": "102"
      },
      {
        "roomType": "DBL",
        "roomId": "201"
      },
      {
        "roomType": "DBL",
        "roomId": "202"
      }
    ]
  }
]

2. bookings.json
[
  {
    "hotelId": "H1",
    "roomId": "101",
    "arrival": "20251115",
    "departure": "20251116",
    "roomType": "DBL",
    "roomRate": "Prepaid"
  },
  {
    "hotelId": "H1",
    "roomId": "101",
    "arrival": "20251115",
    "departure": "20251116",
    "roomType": "SGL",
    "roomRate": "Standard"
  },
  {
    "hotelId": "H1",
    "roomId": "102",
    "arrival": "20251115",
    "departure": "20251116",
    "roomType": "SGL",
    "roomRate": "Standard"
  }
]

