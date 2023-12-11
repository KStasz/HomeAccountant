using HomeAccountant.AccountingService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace HomeAccountant.AccountingService.Services
{
    public class DbContextRepository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public DbContextRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void Add(T entity)
        {
            _applicationDbContext.Set<T>().Add(entity);
        }

        public T? Get(Func<T, bool> predicate)
        {
            return _applicationDbContext.Set<T>().FirstOrDefault(predicate);
        }

        public T? Get(Func<T, bool> predicate, params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> dbSet = _applicationDbContext.Set<T>();

            var result = includeExpressions.Aggregate(dbSet, (x, e) => x.Include(e));

            return result.FirstOrDefault(predicate);
        }

        public IEnumerable<T> GetAll(Func<T, bool> predicate)
        {
            return _applicationDbContext.Set<T>().Where(predicate);
        }

        public IEnumerable<T> GetAll(Func<T, bool> predicate, params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> query = _applicationDbContext.Set<T>();

            var newQuery = includeExpressions.Aggregate(query, (x, e) => x.Include(e));

            return newQuery.Where(predicate);
        }

        public void Remove(T entity)
        {
            _applicationDbContext.Set<T>().Remove(entity);
        }

        public void RemoveMany(IEnumerable<T> entities)
        {
            _applicationDbContext.Set<T>().RemoveRange(entities);
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }

        public EntityEntry Update(T entity)
        {
            return _applicationDbContext.Set<T>().Update(entity);
        }
    }
}
