using NodaTime;

namespace VacationRental.Domain
{
    public class Booking : UnitOccupation
    {
        public Booking(int rentalId, LocalDate startDate, int nights, int unitNumber) 
            : base(rentalId, startDate, nights, unitNumber)
        {
        }
        
        public Booking(int id, int rentalId, LocalDate startDate, int nights, int unitNumber) 
            : base(id, rentalId, startDate, nights, unitNumber)
        {
        }
    }
}