using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Domain
{
    public interface IRepository<T> where T : Entity
    {
        Task AddAsync(T entity);
        IQueryable<T> Table { get; }
        Task<T> GetByIdAsync(Guid id);
        Task SaveAsync(T entity);
        Task SaveAllAsync(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteAllAsync(IEnumerable<T> entities);
        Task SaveChangesAsync(T entity);
        Task SaveChangesAllAsync(IEnumerable<T> entities);
        Task SaveChangesAllWithClearAsync(IEnumerable<T> entities);
        Task SaveChangesWithClearAsync(T entity);

    }
}
