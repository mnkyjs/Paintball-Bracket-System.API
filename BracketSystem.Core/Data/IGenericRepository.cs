using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BracketSystem.Core.Data
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);

        Task AddRange(IEnumerable<T> entities);

        Task Delete(T entityToDelete);

        Task DeleteObjectById(int id);

        Task<IEnumerable<T>> FindByConditionList(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<T> FindByConditionSingle(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<T> GetById(int id);

        Task<IEnumerable<T>> GetWithRawSql(string query,
                                                            params object[] parameters);
        Task RemoveRange(IEnumerable<T> entities);
    }
}