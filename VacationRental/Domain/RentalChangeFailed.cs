using VacationRental.Common;

namespace VacationRental.Domain
{
    public class RentalChangeFailed : DomainError<RentalChangeFailed>
    {
        public RentalChangeFailed(int rentalId)
        {
            Description = $"Failed to change rental '{rentalId}' parameters due to resulting occupation overlapping";
        }
        
        public override string Description { get; }
    }
}