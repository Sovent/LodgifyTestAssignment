namespace VacationRental.Common
{
    public abstract class DomainError<T> where T : DomainError<T>
    {
        public abstract string Description { get; }

        public DomainException<T> ToException() => new DomainException<T>((T)this);
    }
}