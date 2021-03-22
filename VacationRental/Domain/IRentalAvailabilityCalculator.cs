using System.Collections.Generic;
using NodaTime;

namespace VacationRental.Domain
{
    public interface IRentalAvailabilityCalculator
    {
        IReadOnlyCollection<int> GetAvailableUnitNumbers(
            Rental rental,
            IEnumerable<UnitOccupation> rentalOccupations,
            LocalDate startDate,
            int nights);

        IReadOnlyDictionary<LocalDate, IEnumerable<UnitOccupation>> GetOccupationCalendar(
            IEnumerable<UnitOccupation> rentalOccupations,
            LocalDate startDate,
            LocalDate endDate);
    }
}