using NodaTime;

namespace VacationRental.Domain
{
    public class Preparation : UnitOccupation
    {
        public Preparation(int rentalId, LocalDate startDate, int nights, int bookingId, int unitNumber) 
            : base(rentalId, startDate, nights, unitNumber)
        {
            BookingId = bookingId;
        }
        
        public int BookingId { get; }
    }
}