using System.Collections.Generic;
using NodaTime;
using VacationRental.Domain;

namespace VacationRental.Application
{
    public interface IBookingService
    {
        Booking GetBooking(GetBookingQuery query);

        int PlaceBooking(PlaceBookingCommand command);

        IReadOnlyDictionary<LocalDate, IEnumerable<UnitOccupation>> GetBookingCalendar(GetBookingCalendarQuery query);
    }
}