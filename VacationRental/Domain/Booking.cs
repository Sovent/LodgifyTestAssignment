using NodaTime;

namespace VacationRental.Domain
{
    public class Booking : UnitOccupation
    {
        public Booking(int rentalId, LocalDate startDate, int nights, int unitNumber) 
            : base(rentalId, startDate, nights, unitNumber)
        {
        }
    }
}