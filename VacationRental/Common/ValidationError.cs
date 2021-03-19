namespace VacationRental.Common
{
    public class ValidationError : DomainError<ValidationError>
    {
        public ValidationError(string description)
        {
            Description = description;
        }

        public override string Description { get; }
    }
}