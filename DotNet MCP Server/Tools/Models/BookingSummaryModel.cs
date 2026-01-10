using McpServerApp.Services.Travog.Response;

public class Baggage
{
    public string cabin { get; set; }
    public string checkin { get; set; }
}

public class AirInfo
{
    public int? flightId { get; set; }
    public List<Itinerary> flightSegment { get; set; }
}

public class Itinerary
{
    public string from { get; set; }
    public string to { get; set; }
    public string date { get; set; }
    public string airline { get; set; }
    public string flight { get; set; }
    public string cabin { get; set; }
}

public class Passenger
{
    public string name { get; set; }
    public string type { get; set; }
}

public class BookingSummary
{
    public string booking_id { get; set; }
    public string status { get; set; }
    public string trip_type { get; set; }
    public List<Passenger> passengers { get; set; }
    public AirInfo airInfo { get; set; }
    public Baggage baggage { get; set; }
    public bool change_allowed { get; set; }
    public bool cancellation_allowed { get; set; }
    public string change_penalty { get; set; }
    public string seat_selection { get; set; }
    public string meals { get; set; }

    // 👇 Factory method
    public static BookingSummary FromDetail(BookingDetailResponse response)
    {
        var booking = response.data.bookingDetails;

        return new BookingSummary
        {
            booking_id = booking.masterDetails.bookingRef.ToString(),
            status = booking.masterDetails.status,

            passengers = booking.passengerInfo.pax
                .Select(p => new Passenger
                {
                    name = $"{p.firstName} {p.lastName}",
                    type = p.type
                })
                .ToList(),

            airInfo = new AirInfo
            {
                flightId = booking.products.airInfo.FirstOrDefault()?.flightId,
                flightSegment = booking.products.airInfo
                    .SelectMany(a => a.flightSegment)
                    .Select(f => new Itinerary
                    {
                        from = f.depAirportCode,
                        to = f.arrAirportCode,
                        date = f.depDate,
                        airline = f.airlineCode,
                        flight = f.flightNumber,
                        cabin = f.@class
                    })
                    .ToList()
            },
            baggage = new Baggage
            {
                cabin = "Included",
                checkin = "Included"
            },
            change_allowed = true,
            cancellation_allowed = true,
            change_penalty = "As per airline",
            seat_selection = "Available",
            meals = "Available"
        };
    }
}