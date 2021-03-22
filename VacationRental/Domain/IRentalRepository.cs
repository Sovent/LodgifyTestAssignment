namespace VacationRental.Domain
{
    public interface IRentalRepository
    {
        Rental GetById(int id);

        void Save(Rental rental);
    }
}