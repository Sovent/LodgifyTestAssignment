using FluentValidation;
using VacationRental.Common;

namespace VacationRental.Application
{
    public class CreateRentalCommand 
        : IValidatable<CreateRentalCommand, CreateRentalCommand.CreateRentalCommandValidator>
    {
        public CreateRentalCommand(int units)
        {
            Units = units;
            
            this.Validate();
        }

        public int Units { get; }

        private class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
        {
            public CreateRentalCommandValidator()
            {
                RuleFor(command => command.Units).GreaterThan(0);
            }
        }
    }
}