using FluentValidation;

namespace VacationRental.Common
{
    public interface IValidatable<TEntity, TValidator> 
        where TValidator : AbstractValidator<TEntity>, new()
        where TEntity : IValidatable<TEntity, TValidator>
    {
    }
}