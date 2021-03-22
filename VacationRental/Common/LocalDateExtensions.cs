using NodaTime;

namespace VacationRental.Common
{
    public static class LocalDateExtensions
    {
        public static LocalDate LastDayAfterSpentNights(this LocalDate dateTime, int nights) => 
            dateTime.PlusDays(nights - 1);
    }
}