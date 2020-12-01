using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace il_y.BracketSystem.Core.Data
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);

        Task<IEnumerable<T>> GetWithRawSql(string query,
            params object[] parameters);

        Task AddRange(IEnumerable<T> entities);

        Task<IEnumerable<T>> FindByConditionList(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<T> FindByConditionSingle(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<T> GetById(int id);

        Task Delete(T entityToDelete);

        Task DeleteObjectById(int id);

        Task RemoveRange(IEnumerable<T> entities);
    }
}