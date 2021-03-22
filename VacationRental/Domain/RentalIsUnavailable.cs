using NodaTime;
using VacationRental.Common;

namespace VacationRental.Domain
{
    public class RentalIsUnavailable : DomainError<RentalIsUnavailable>
    {
        public RentalIsUnavailable(int rentalId, LocalDate startDate, int nights)
        {
            Description = $"Rental with id '{rentalId}' is not available to rent from {startDate:d} on {nights} nights";
        }

        public override string Description { get; }
    }
}