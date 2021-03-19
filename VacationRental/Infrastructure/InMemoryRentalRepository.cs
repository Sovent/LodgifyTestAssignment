using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VacationRental.Common;
using VacationRental.Domain;

namespace VacationRental.Infrastructure
{
    public class InMemoryRentalRepository : IRentalRepository
    {
        public Task<Rental> GetById(int id)
        {
            var rental =  _rentals.GetValueOrDefault(id);
            if (rental == default)
            {
                throw new RentalNotFound(id).ToException();
            }

            return Task.FromResult(rental);
        }

        // note: int id means sequential id generation typical for relational databases
        public Task Save(Rental rental)
        {
            lock (_lock)
            {
                var newId = _rentals.Any() ? _rentals.Keys.Max() + 1 : 1;
                _rentals[newId] = rental;
                _rentalIdProperty.SetValue(rental, newId);
                return Task.CompletedTask;
            }
        }

        private readonly PropertyInfo _rentalIdProperty =
            typeof(Rental).GetProperty(nameof(Rental.Id));
        private readonly Dictionary<int, Rental> _rentals = new Dictionary<int, Rental>();
        private readonly object _lock = new object();
    }
}