using FluentValidation;
using NodaTime;
using VacationRental.Common;

namespace VacationRental.Application
{
    public class PlaceBookingCommand 
        : IValidatable<PlaceBookingCommand, PlaceBookingCommand.PlaceBookingCommandValidator>
    {
        public PlaceBookingCommand(int rentalId, LocalDate startDate, int nights)
        {
            RentalId = rentalId;
            StartDate = startDate;
            Nights = nights;
            
            this.Validate();
        }

        public int RentalId { get; }
        
        public LocalDate StartDate { get; }
        
        public int Nights { get; }

        private class PlaceBookingCommandValidator : AbstractValidator<PlaceBookingCommand>
        {
            public PlaceBookingCommandValidator()
            {
                RuleFor(command => command.RentalId).GreaterThan(0);
                RuleFor(command => command.Nights).GreaterThan(0);
            }
        }
    }
}