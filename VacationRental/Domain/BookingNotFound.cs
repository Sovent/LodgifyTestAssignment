using VacationRental.Common;

namespace VacationRental.Domain
{
    public class BookingNotFound : DomainError<BookingNotFound>
    {
        public BookingNotFound(int bookingId)
        {
            Description = $"Booking with id {bookingId} not found";
        }

        public override string Description { get; }
    }
}