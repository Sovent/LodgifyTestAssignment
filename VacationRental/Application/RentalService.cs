using VacationRental.Domain;

namespace VacationRental.Application
{
    public class RentalService : IRentalService
    {
        public RentalService(IRentalRepository rentalRepository)
        {
            _rentalRepository = rentalRepository;
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
        
        private readonly IRentalRepository _rentalRepository;
    }
}