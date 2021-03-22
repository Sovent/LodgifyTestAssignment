using System.Collections.Generic;
using System.Linq;
using NodaTime;
using VacationRental.Domain;

namespace VacationRental.Application
{
    public class BookingService : IBookingService
    {
        public BookingService(
            IRentalRepository rentalRepository,
            IUnitOccupationRepository occupationRepository,
            IRentalAvailabilityCalculator availabilityCalculator,
            IPreparationScheduler preparationScheduler)
        {
            _rentalRepository = rentalRepository;
            _occupationRepository = occupationRepository;
            _availabilityCalculator = availabilityCalculator;
            _preparationScheduler = preparationScheduler;
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
            var bookingStartDate = command.StartDate;
            var nightsCountToBook = command.Nights;
            var availableUnitNumbers = _availabilityCalculator.GetAvailableUnitNumbers(
                rental,
                occupations,
                bookingStartDate,
                nightsCountToBook);
            if (!availableUnitNumbers.Any())
            {
                throw new RentalIsUnavailable(rentalId, bookingStartDate, nightsCountToBook).ToException();
            }
            
            var booking = new Booking(rentalId, bookingStartDate, nightsCountToBook, availableUnitNumbers.First());
            _occupationRepository.Save(booking);
            _preparationScheduler.SchedulePreparationAfterBooking(rental, booking);
            return booking.Id;
        }

        public IReadOnlyDictionary<LocalDate, IEnumerable<UnitOccupation>> GetBookingCalendar(
            GetBookingCalendarQuery query)
        {
            var rental = _rentalRepository.GetById(query.RentalId);
            var occupations = _occupationRepository.GetForRental(rental.Id);
            return _availabilityCalculator.GetOccupationCalendar(occupations, query.StartDate, query.EndDate);
        }

        private readonly IRentalRepository _rentalRepository;
        private readonly IUnitOccupationRepository _occupationRepository;
        private readonly IRentalAvailabilityCalculator _availabilityCalculator;
        private readonly IPreparationScheduler _preparationScheduler;
    }
}