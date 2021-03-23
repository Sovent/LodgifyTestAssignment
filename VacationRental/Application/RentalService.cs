using VacationRental.Domain;

namespace VacationRental.Application
{
    public class RentalService : IRentalService
    {
        public RentalService(IRentalRepository rentalRepository, IRentalChangeProcessor rentalChangeProcessor)
        {
            _rentalRepository = rentalRepository;
            _rentalChangeProcessor = rentalChangeProcessor;
        }
        
        public int CreateRental(CreateRentalCommand command)
        {
            var rental = new Rental(command.Units, command.PreparationPeriod);
            _rentalRepository.Save(rental);

            return rental.Id;
        }

        public Rental GetRental(GetRentalQuery query)
        {
            return _rentalRepository.GetById(query.Id);
        }

        public void ChangeRental(ChangeRentalCommand command)
        {
            var rental = _rentalRepository.GetById(command.RentalId);
            var isChanged = rental.TryChange(command.Units, command.PreparationPeriod);
            if (!isChanged)
            {
                return;
            }
            
            _rentalChangeProcessor.RescheduleOccupationsForNewRentalParameters(rental);
            _rentalRepository.Save(rental);
        }

        private readonly IRentalRepository _rentalRepository;
        private readonly IRentalChangeProcessor _rentalChangeProcessor;
    }
}