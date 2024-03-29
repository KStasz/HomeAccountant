﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Domain.Services
{
    public class DbContextRepository<U, T> : IRepository<U, T> where T : class where U : DbContext
    {
        private readonly U _applicationDbContext;

        public DbContextRepository(U applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public EntityEntry<T>? Add(T entity)
        {
            if (entity is null)
                return null;

            return _applicationDbContext.Set<T>().Add(entity);
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

        public EntityEntry<T>? Remove(T entity)
        {
            return _applicationDbContext.Set<T>().Remove(entity);
        }

        public void RemoveMany(IEnumerable<T> entities)
        {
            _applicationDbContext.Set<T>().RemoveRange(entities);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _applicationDbContext.SaveChangesAsync();
        }

        public EntityEntry<T>? Update(T entity)
        {
            return _applicationDbContext.Set<T>().Update(entity);
        }
    }
}
