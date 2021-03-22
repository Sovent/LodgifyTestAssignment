using System.Collections.Generic;

namespace VacationRental.Domain
{
    public interface IUnitOccupationRepository
    {
        Booking GetBookingById(int id);

        IEnumerable<UnitOccupation> GetForRental(int rentalId);

        void Save(UnitOccupation occupation);
    }
}