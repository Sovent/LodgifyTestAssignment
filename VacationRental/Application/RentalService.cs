using System.Threading.Tasks;
using VacationRental.Domain;

namespace VacationRental.Application
{
    public class RentalService : IRentalService
    {
        public RentalService(IRentalRepository rentalRepository)
        {
            _rentalRepository = rentalRepository;
        }
        
        public async Task<int> CreateRental(CreateRentalCommand command)
        {
            var rental = new Rental(command.Units);
            await _rentalRepository.Save(rental);

            return rental.Id;
        }

        public async Task<Rental> GetRental(GetRentalQuery query)
        {
            return await _rentalRepository.GetById(query.Id);
        }
        
        private readonly IRentalRepository _rentalRepository;
    }
}