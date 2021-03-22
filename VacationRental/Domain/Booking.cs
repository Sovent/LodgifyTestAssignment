using NodaTime;

namespace VacationRental.Domain
{
    public class Booking : UnitOccupation
    {
        public Booking(int rentalId, LocalDate startDate, int nights) : base(rentalId, startDate, nights)
        {
        }
    }
}