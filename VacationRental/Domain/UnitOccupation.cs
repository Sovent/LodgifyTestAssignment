using System.Collections.Generic;
using System.Linq;
using NodaTime;
using VacationRental.Common;

namespace VacationRental.Domain
{
    public abstract class UnitOccupation : IEntity
    {
        protected UnitOccupation(int rentalId, LocalDate startDate, int nights)
        {
            StartDate = startDate;
            Nights = nights;
            RentalId = rentalId;
        }

        public int Id { get; private set; }
        
        public int RentalId { get; }
        
        public LocalDate StartDate { get; }
        
        public int Nights { get; }

        public LocalDate EndDate => StartDate.LastDayAfterSpentNights(Nights);
        public IEnumerable<LocalDate> OccupiedDays =>
            Enumerable.Range(0, Nights).Select(night => StartDate.PlusDays(night));
    }
}