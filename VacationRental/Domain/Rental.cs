using VacationRental.Common;

namespace VacationRental.Domain
{
    public class Rental : IEntity
    {
        public Rental(int units)
        {
            Units = units;
        }
        
        public int Id { get; private set; }
        
        public int Units { get; }
    }
}