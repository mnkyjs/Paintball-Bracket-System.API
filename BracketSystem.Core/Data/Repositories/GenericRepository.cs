using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace il_y.BracketSystem.Core.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        protected readonly DbContext DbContext;

        public GenericRepository(DbContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetWithRawSql(string query,
            params object[] parameters)
        {
            return await _dbSet.FromSqlRaw(query, parameters).ToListAsync();
        }

        public void Add(T entity)
        {
            _dbSet.AddAsync(entity);
        }

        public virtual Task AddRange(IEnumerable<T> entities)
        {
            return _dbSet.AddRangeAsync(entities);
        }

        public virtual async Task<IEnumerable<T>> FindByConditionList(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null) query = query.Where(filter);

            if (include != null) query = include(query);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public virtual async Task<T> FindByConditionSingle(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null) query = query.Where(filter);

            if (include != null) query = include(query);

            if (orderBy != null)
                return await orderBy(query).FirstOrDefaultAsync();
            return await query.FirstOrDefaultAsync();
        }
        public virtual async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public Task Delete(T entityToDelete)
        {
            if (DbContext.Entry(entityToDelete).State == EntityState.Detached) _dbSet.Attach(entityToDelete);

            return Task.Run(() => _dbSet.Remove(entityToDelete));
        }

        public virtual async Task DeleteObjectById(int id)
        {
            var entityToDelete = await _dbSet.FindAsync(id);
            await Delete(entityToDelete);
        }

        public virtual Task RemoveRange(IEnumerable<T> entities)
        {
            return Task.Run(() => _dbSet.RemoveRange(entities));
        }
    }
}