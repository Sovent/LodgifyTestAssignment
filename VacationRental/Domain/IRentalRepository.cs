using System.Threading.Tasks;

namespace VacationRental.Domain
{
    public interface IRentalRepository
    {
        Task<Rental> GetById(int id);

        Task Save(Rental rental);
    }
}