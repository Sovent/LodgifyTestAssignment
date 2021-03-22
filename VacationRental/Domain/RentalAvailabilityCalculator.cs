using System.Collections.Generic;
using System.Linq;
using NodaTime;
using VacationRental.Common;

namespace VacationRental.Domain
{
    public class RentalAvailabilityCalculator : IRentalAvailabilityCalculator
    {
        public void CheckAvailability(
            Rental rental,
            IEnumerable<UnitOccupation> rentalOccupations,
            LocalDate startDate,
            int nights)
        {
            var endDate = startDate.LastDayAfterSpentNights(nights);
            var occupationSchedule = GetOccupationSchedule(rentalOccupations, startDate, endDate);
            var maxUnitsOccupied = occupationSchedule.Max(occupationsByDay => occupationsByDay.Value.Count());
            if (maxUnitsOccupied >= rental.Units)
            {
                throw new RentalIsUnavailable(rental.Id, startDate, nights).ToException();
            }
        }

        public IReadOnlyDictionary<LocalDate, IEnumerable<UnitOccupation>> GetOccupationSchedule(
            IEnumerable<UnitOccupation> rentalOccupations,
            LocalDate startDate,
            LocalDate endDate)
        {
            var occupationsIntersectingWithPeriod = rentalOccupations.Where(
                occupation => occupation.EndDate >= startDate && occupation.StartDate <= endDate);
            var occupationsIntersectingWithPeriodByDay = occupationsIntersectingWithPeriod
                .SelectMany(occupation => occupation.OccupiedDays.Select(occupiedDay => new {occupiedDay, occupation}))
                .ToLookup(pair => pair.occupiedDay, pair => pair.occupation);
            var schedule = new Dictionary<LocalDate, IEnumerable<UnitOccupation>>();
            for (var date = startDate; date <= endDate; date = date.PlusDays(1))
            {
                schedule[date] = occupationsIntersectingWithPeriodByDay[date];
            }

            return schedule;
        }
    }
}