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
        
        public int Units { get; private set; }
        
        public Period PreparationPeriod { get; private set; }

        public bool TryChange(int newUnits, Period newPreparationPeriod)
        {
            var changed = false;
            if (Units != newUnits)
            {
                Units = newUnits;
                changed = true;
            }

            if (PreparationPeriod != newPreparationPeriod)
            {
                PreparationPeriod = newPreparationPeriod;
                changed = true;
            }

            return changed;
        }
    }
}