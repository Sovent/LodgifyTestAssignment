using System.Collections.Generic;
using NodaTime;
using VacationRental.Domain;

namespace VacationRental.Application
{
    public class BookingService : IBookingService
    {
        public BookingService(
            IRentalRepository rentalRepository,
            IUnitOccupationRepository occupationRepository,
            IRentalAvailabilityCalculator availabilityCalculator)
        {
            _rentalRepository = rentalRepository;
            _occupationRepository = occupationRepository;
            _availabilityCalculator = availabilityCalculator;
        }
        
        public Booking GetBooking(GetBookingQuery query)
        {
            return _occupationRepository.GetBookingById(query.BookingId);
        }

        public int PlaceBooking(PlaceBookingCommand command)
        {
            var rentalId = command.RentalId;
            var rental = _rentalRepository.GetById(rentalId);
            var occupations = _occupationRepository.GetForRental(rentalId);
            _availabilityCalculator.CheckAvailability(rental, occupations, command.StartDate, command.Nights);
            var booking = new Booking(rentalId, command.StartDate, command.Nights);
            _occupationRepository.Save(booking);
            return booking.Id;
        }

        public IReadOnlyDictionary<LocalDate, IEnumerable<UnitOccupation>> GetBookingCalendar(
            GetBookingCalendarQuery query)
        {
            var rental = _rentalRepository.GetById(query.RentalId);
            var occupations = _occupationRepository.GetForRental(rental.Id);
            return _availabilityCalculator.GetOccupationSchedule(occupations, query.StartDate, query.EndDate);
        }

        private readonly IRentalRepository _rentalRepository;
        private readonly IUnitOccupationRepository _occupationRepository;
        private readonly IRentalAvailabilityCalculator _availabilityCalculator;
    }
}