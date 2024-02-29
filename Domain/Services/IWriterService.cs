using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Domain.Services
{
    public interface IWriterService<T> where T : class
    {
        EntityEntry<T>? Add(T entity);
        EntityEntry<T>? Update(T entity);
        EntityEntry<T>? Remove(T entity);
        void RemoveMany(IEnumerable<T> entities);
    }
}
