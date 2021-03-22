using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VacationRental.Common;
using VacationRental.Domain;

namespace VacationRental.Infrastructure
{
    public class InMemoryRentalRepository : InMemoryRepository<Rental>, IRentalRepository
    {
        public Rental GetById(int id)
        {
            var rental = TryGetById(id);
            if (rental == default)
            {
                throw new RentalNotFound(id).ToException();
            }

            return rental;
        }

        void IRentalRepository.Save(Rental rental)
        {
            base.Save(rental);
        }
    }
}