using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VacationRental.Common;
using VacationRental.Domain;

namespace VacationRental.Infrastructure
{
    public abstract class InMemoryRepository<T> where T : IEntity
    {
        protected T TryGetById(int id)
        {
            return _entities.GetValueOrDefault(id);
        }

        // note: int id means sequential id generation typical for relational databases
        protected void Save(T entity)
        {
            var newId = _entities.Any() ? _entities.Keys.Max() + 1 : 1;
            _entities[newId] = entity;
            _idProperty.SetValue(entity, newId);
        }

        protected IEnumerable<T> GetAll()
        {
            return _entities.Values;
        }
        
        private readonly PropertyInfo _idProperty = typeof(T).GetProperty(nameof(IEntity.Id));
        private readonly Dictionary<int, T> _entities = new Dictionary<int, T>();
    }
}