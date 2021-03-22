using System.Collections.Generic;
using System.Linq;
using NodaTime;
using VacationRental.Common;

namespace VacationRental.Domain
{
    public class RentalAvailabilityCalculator : IRentalAvailabilityCalculator
    {
        public IReadOnlyCollection<int> GetAvailableUnitNumbers(
            Rental rental,
            IEnumerable<UnitOccupation> rentalOccupations,
            LocalDate startDate,
            int nights)
        {
            var endDate = startDate.LastDayAfterSpentNights(nights + rental.PreparationPeriod.Days);
            var occupationCalendar = GetOccupationCalendar(rentalOccupations, startDate, endDate);
            var occupiedUnitsDuringThePeriod =
                occupationCalendar.Values.Flatten().Select(occupation => occupation.UnitNumber);
            var unitsToCheckAvailabilityFor = Enumerable.Range(1, rental.Units);
            return unitsToCheckAvailabilityFor.Except(occupiedUnitsDuringThePeriod).ToArray();
        }

        public IReadOnlyDictionary<LocalDate, IEnumerable<UnitOccupation>> GetOccupationCalendar(
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