using System.Threading.Tasks;
using VacationRental.Domain;

namespace VacationRental.Application
{
    public interface IRentalService
    {
        Task<int> CreateRental(CreateRentalCommand command);

        Task<Rental> GetRental(GetRentalQuery query);
    }
}