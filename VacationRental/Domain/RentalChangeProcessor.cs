using System.Linq;
using VacationRental.Common;

namespace VacationRental.Domain
{
    public class RentalChangeProcessor : IRentalChangeProcessor
    {
        public RentalChangeProcessor(
            IUnitOccupationRepository unitOccupationRepository,
            IRentalAvailabilityCalculator rentalAvailabilityCalculator,
            IPreparationScheduler preparationScheduler)
        {
            _unitOccupationRepository = unitOccupationRepository;
            _rentalAvailabilityCalculator = rentalAvailabilityCalculator;
            _preparationScheduler = preparationScheduler;
        }
        
        public void RescheduleOccupationsForNewRentalParameters(Rental rental)
        {
            var occupationsForRental = _unitOccupationRepository.GetForRental(rental.Id).ToArray();
            var bookings = occupationsForRental.OfType<Booking>().ToArray();
            var afterBookingPreparations = occupationsForRental
                .OfType<Preparation>()
                .ToDictionary(preparation => preparation.BookingId);
            foreach (UnitOccupation booking in bookings)
            {
                var exclusionsFromAvailabilityCheck =
                    new[] {booking, afterBookingPreparations.GetValueOrDefault(booking.Id)}
                        .Where(occupation => occupation != default);
                var availableUnitNumbers = _rentalAvailabilityCalculator.GetAvailableUnitNumbers(
                    rental,
                    occupationsForRental.Except(exclusionsFromAvailabilityCheck),
                    booking.StartDate,
                    booking.Nights);
                if (!availableUnitNumbers.Contains(booking.UnitNumber))
                {
                    throw new RentalChangeFailed(rental.Id).ToException();
                }
            }

            foreach (var booking in bookings)
            {
                var afterBookingPreparation = afterBookingPreparations[booking.Id];
                _unitOccupationRepository.Remove(afterBookingPreparation.Id);
                _preparationScheduler.SchedulePreparationAfterBooking(rental, booking);
            }
        }
        
        private readonly IUnitOccupationRepository _unitOccupationRepository;
        private readonly IRentalAvailabilityCalculator _rentalAvailabilityCalculator;
        private readonly IPreparationScheduler _preparationScheduler;
    }
}