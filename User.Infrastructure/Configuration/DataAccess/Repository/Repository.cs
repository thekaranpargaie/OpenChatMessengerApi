using Base.Domain;
using Microsoft.EntityFrameworkCore;

namespace User.Infrastructure.Configuration.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        protected UserDb DbContext { get; set; }
        protected DbSet<T> DbSet => DbContext.Set<T>();

        public Repository(UserDb dbContext)
        {
            DbContext = dbContext;
        }
        public async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
        }
        public IQueryable<T> Table
        {
            get
            {
                return DbSet.AsNoTracking();
            }
        }
        public async Task<T> GetByIdAsync(Guid id)
        {
            return await DbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task DeleteAsync(T entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            await Task.Run(() => DbSet.Remove(entity));
        }

        public async Task SaveAsync(T entity)
        {
            if (entity.IsNew)
            {
                await DbSet.AddAsync(entity);
            }
            else
            {

                await Task.Run(() => DbSet.Update(entity));
            }
        }

        public async Task SaveAllAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.IsNew)
                {
                    await DbSet.AddAsync(entity);
                }
                else
                {
                    await Task.Run(() => DbSet.Update(entity));
                }
            }

        }
        public async Task SaveChangesAsync(T entity)
        {
            if (entity.IsNew)
            {
                await DbSet.AddAsync(entity);
            }
            else
            {

                await Task.Run(() => DbSet.Update(entity));
            }
            await DbContext.SaveChangesAsync();
        }
        public async Task DeleteAllAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                DbSet.RemoveRange(entity);
            }
            await DbContext.SaveChangesAsync();
        }

        public async Task SaveChangesAllAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.IsNew)
                {
                    await DbSet.AddAsync(entity);
                }
                else
                {
                    await Task.Run(() => DbSet.Update(entity));
                }
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task SaveChangesWithClearAsync(T entity)
        {
            if (entity.IsNew)
            {
                await DbSet.AddAsync(entity);
            }
            else
            {

                await Task.Run(() => DbSet.Update(entity));
            }
            await DbContext.SaveChangesWithClearAsync();
        }

        public async Task SaveChangesAllWithClearAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.IsNew)
                {
                    await DbSet.AddAsync(entity);
                }
                else
                {
                    await Task.Run(() => DbSet.Update(entity));
                }
            }

            await DbContext.SaveChangesWithClearAsync();
        }
    }
}
