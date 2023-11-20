using HomeAccountant.AccountingService.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace HomeAccountant.AccountingService.Services
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        EntityEntry Update(T entity);
        T? Get(Func<T, bool> predicate);
        public T? Get(Func<T, bool> predicate, Expression<Func<T, object>>[] includeExpressions);
        IEnumerable<T> GetAll(Func<T, bool> predicate);
        IEnumerable<T> GetAll(Func<T, bool> predicate, Expression<Func<T, object>>[] includeExpressions);
        void Remove(T entity);
        Task SaveChangesAsync();
    }
}
