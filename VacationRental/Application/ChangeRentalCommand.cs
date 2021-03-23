using FluentValidation;
using NodaTime;
using VacationRental.Common;

namespace VacationRental.Application
{
    public class ChangeRentalCommand 
        : IValidatable<ChangeRentalCommand, ChangeRentalCommand.ChangeRentalCommandValidator>
    {
        public ChangeRentalCommand(int rentalId, int units, Period preparationPeriod)
        {
            RentalId = rentalId;
            Units = units;
            PreparationPeriod = preparationPeriod;
            
            this.Validate();
        }

        public int RentalId { get; }
        
        public int Units { get; }
        
        public Period PreparationPeriod { get; }

        private class ChangeRentalCommandValidator : AbstractValidator<ChangeRentalCommand>
        {
            public ChangeRentalCommandValidator()
            {
                RuleFor(command => command.RentalId).GreaterThan(0);
                RuleFor(command => command.Units).GreaterThan(0);
                RuleFor(command => command.PreparationPeriod).Must(period => !period.HasTimeComponent);
            }
        }
    }
}