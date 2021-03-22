using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace VacationRental.Domain
{
    public interface IRentalAvailabilityCalculator
    {
        void CheckAvailability(
            Rental rental,
            IEnumerable<UnitOccupation> rentalOccupations,
            LocalDate startDate,
            int nights);

        IReadOnlyDictionary<LocalDate, IEnumerable<UnitOccupation>> GetOccupationSchedule(
            IEnumerable<UnitOccupation> rentalOccupations,
            LocalDate startDate,
            LocalDate endDate);
    }
}