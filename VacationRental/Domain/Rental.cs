using NodaTime;
using VacationRental.Common;

namespace VacationRental.Domain
{
    public class Rental : IEntity
    {
        public Rental(int units, Period preparationPeriod)
        {
            Units = units;
            PreparationPeriod = preparationPeriod;
        }
        
        public int Id { get; private set; }
        
        public int Units { get; }
        
        public Period PreparationPeriod { get; }
    }
}