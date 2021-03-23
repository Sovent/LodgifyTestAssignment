using VacationRental.Domain;

namespace VacationRental.Application
{
    public interface IRentalService
    {
        int CreateRental(CreateRentalCommand command);

        Rental GetRental(GetRentalQuery query);

        void ChangeRental(ChangeRentalCommand command);
    }
}