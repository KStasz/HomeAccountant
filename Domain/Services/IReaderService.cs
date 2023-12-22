using System.Linq.Expressions;

namespace Domain.Services
{
    public interface IReaderService<T> where T : class
    {
        T? Get(Func<T, bool> predicate);
        public T? Get(Func<T, bool> predicate, params Expression<Func<T, object>>[] includeExpressions);
        IEnumerable<T> GetAll(Func<T, bool> predicate);
        IEnumerable<T> GetAll(Func<T, bool> predicate, params Expression<Func<T, object>>[] includeExpressions);
    }
}
