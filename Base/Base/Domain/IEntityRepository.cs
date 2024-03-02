namespace Base.Domain
{
    public interface IEntityRepository<T> where T : DomainBase
    {
        Task AddAsync(T entity);
        IQueryable<T> Table { get; }
        Task<T> GetByIdAsync(long id);
        Task SaveAsync(T entity);
        Task SaveAllAsync(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync(T entity);

    }
}
