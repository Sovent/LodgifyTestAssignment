namespace VacationRental.Domain
{
    public class Rental
    {
        public Rental(int units)
        {
            Units = units;
        }
        
        public int Id { get; private set; }
        
        public int Units { get; }
    }
}