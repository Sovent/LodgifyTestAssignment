using FluentValidation;
using VacationRental.Common;

namespace VacationRental.Application
{
    public class GetBookingQuery : IValidatable<GetBookingQuery, GetBookingQuery.GetBookingQueryValidator>
    {
        public GetBookingQuery(int bookingId)
        {
            BookingId = bookingId;
            
            this.Validate();
        }

        public int BookingId { get; }

        private class GetBookingQueryValidator : AbstractValidator<GetBookingQuery>
        {
            public GetBookingQueryValidator()
            {
                RuleFor(query => query.BookingId).GreaterThan(0);
            }
        }
    }
}