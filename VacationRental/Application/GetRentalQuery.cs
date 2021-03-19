using FluentValidation;
using VacationRental.Common;

namespace VacationRental.Application
{
    public class GetRentalQuery : IValidatable<GetRentalQuery, GetRentalQuery.GetRentalQueryValidator>
    {
        public GetRentalQuery(int id)
        {
            Id = id;

            this.Validate();
        }

        public int Id { get; }

        private class GetRentalQueryValidator : AbstractValidator<GetRentalQuery>
        {
            public GetRentalQueryValidator()
            {
                RuleFor(query => query.Id).GreaterThan(0);
            }
        }
    }
}