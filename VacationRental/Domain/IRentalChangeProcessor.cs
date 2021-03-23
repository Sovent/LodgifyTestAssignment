namespace VacationRental.Domain
{
    public interface IRentalChangeProcessor
    {
        void RescheduleOccupationsForNewRentalParameters(Rental rental);
    }
}