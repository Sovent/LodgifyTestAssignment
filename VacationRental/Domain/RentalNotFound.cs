using VacationRental.Common;

namespace VacationRental.Domain
{
    public class RentalNotFound : DomainError<RentalNotFound>
    {
        public RentalNotFound(int rentalId)
        {
            Description = $"Rental with id {rentalId} not found";
        }
        
        public override string Description { get; }
    }
}