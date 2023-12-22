using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.Services
{
    public interface IWriterService<T> where T : class
    {
        void Add(T entity);
        EntityEntry Update(T entity);
        void Remove(T entity);
        void RemoveMany(IEnumerable<T> entities);
    }
}
