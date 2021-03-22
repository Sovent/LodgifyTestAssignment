using FluentValidation;
using NodaTime;
using VacationRental.Common;

namespace VacationRental.Application
{
    public class CreateRentalCommand 
        : IValidatable<CreateRentalCommand, CreateRentalCommand.CreateRentalCommandValidator>
    {
        public CreateRentalCommand(int units, Period preparationPeriod)
        {
            Units = units;
            PreparationPeriod = preparationPeriod;

            this.Validate();
        }

        public int Units { get; }

        public Period PreparationPeriod { get; }
        
        private class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
        {
            public CreateRentalCommandValidator()
            {
                RuleFor(command => command.Units).GreaterThan(0);
                RuleFor(command => command.PreparationPeriod).Must(period => !period.HasTimeComponent);
            }
        }
    }
}