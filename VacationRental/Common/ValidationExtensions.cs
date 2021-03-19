using FluentValidation;

namespace VacationRental.Common
{
    public static class ValidationExtensions
    {
        public static void Validate<TEntity, TValidator>(this IValidatable<TEntity, TValidator> validatable)
            where TEntity : IValidatable<TEntity, TValidator> 
            where TValidator : AbstractValidator<TEntity>, new()
        {
            var validator = new TValidator();
            var validationResult = validator.Validate((TEntity)validatable);
            if (!validationResult.IsValid)
            {
                throw new ValidationError(validationResult.ToString()).ToException();
            }
        }
    }
}