using FluentValidation;
using NodaTime;
using VacationRental.Common;

namespace VacationRental.Application
{
    public class GetBookingCalendarQuery 
        : IValidatable<GetBookingCalendarQuery, GetBookingCalendarQuery.GetBookingCalendarQueryValidator>
    {
        public GetBookingCalendarQuery(int rentalId, LocalDate startDate, LocalDate endDate)
        {
            RentalId = rentalId;
            StartDate = startDate;
            EndDate = endDate;

            this.Validate();
        }

        public int RentalId { get; }
        
        public LocalDate StartDate { get; }
        
        public LocalDate EndDate { get; }

        private class GetBookingCalendarQueryValidator : AbstractValidator<GetBookingCalendarQuery>
        {
            public GetBookingCalendarQueryValidator()
            {
                RuleFor(query => query.RentalId).GreaterThan(0);
            }
        }
    }
}