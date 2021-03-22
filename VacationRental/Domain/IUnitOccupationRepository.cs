using System.Collections.Generic;
using System.Linq;

namespace VacationRental.Domain
{
    public interface IUnitOccupationRepository
    {
        Booking GetBookingById(int id);

        IEnumerable<UnitOccupation> GetForRental(int rentalId);

        void Save(UnitOccupation occupation);
    }
}