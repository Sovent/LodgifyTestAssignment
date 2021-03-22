using NodaTime;

namespace VacationRental.Domain
{
    public class PreparationScheduler : IPreparationScheduler
    {
        private readonly IUnitOccupationRepository _unitOccupationRepository;

        public PreparationScheduler(IUnitOccupationRepository unitOccupationRepository)
        {
            _unitOccupationRepository = unitOccupationRepository;
        }

        public void SchedulePreparationAfterBooking(Rental rental, Booking booking)
        {
            if (rental.PreparationPeriod == Period.Zero)
            {
                return;
            }

            var preparationStartDate = booking.EndDate.PlusDays(1);
            var preparation = new Preparation(
                rental.Id,
                preparationStartDate,
                rental.PreparationPeriod.Days,
                booking.Id,
                booking.UnitNumber);
            _unitOccupationRepository.Save(preparation);
        }
    }
}