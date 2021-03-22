namespace VacationRental.Domain
{
    public interface IPreparationScheduler
    {
        void SchedulePreparationAfterBooking(Rental rental, Booking booking);
    }
}