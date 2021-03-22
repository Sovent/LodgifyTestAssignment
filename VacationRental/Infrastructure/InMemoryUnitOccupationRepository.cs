using System.Collections.Generic;
using VacationRental.Domain;

namespace VacationRental.Infrastructure
{
    public class InMemoryUnitOccupationRepository : InMemoryRepository<UnitOccupation>, IUnitOccupationRepository
    {
        public Booking GetBookingById(int id)
        {
            var booking = TryGetById(id) as Booking;
            if (booking == default)
            {
                throw new BookingNotFound(id).ToException();
            }

            return booking;
        }

        public IEnumerable<UnitOccupation> GetForRental(int rentalId) => GetAll();

        void IUnitOccupationRepository.Save(UnitOccupation occupation) => base.Save(occupation);
    }
}