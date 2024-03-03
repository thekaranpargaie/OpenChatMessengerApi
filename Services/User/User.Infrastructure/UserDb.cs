using Base.Domain;
using Base.Logging;
using Base.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace User.Infrastructure
{
    public class UserDb : DbContext
    {
        internal const string SchemaName = "User";
        public DbSet<Domain.User> Users { get; set; }
        public DbSet<Domain.UserAccount> UserAccounts { get; set; }
        public void InitDb()
        {
            Database.GetPendingMigrations();
            Database.Migrate();
        }
        readonly SessionManager _sessionManager;
        readonly IDebugLogger _log;
        public UserDb(DbContextOptions<UserDb> options, SessionManager sessionManager, IDebugLogger log) : base(options)
        {
            _sessionManager = sessionManager;
            _log = log;
            InitDb();
        }

        public async Task<int> SaveChangesWithClearAsync(CancellationToken cancellationToken = default)
        {
            UpdateOperationsLog(ChangeTracker);
            HandleSoftDelete(ChangeTracker);
            var operationResult = await base.SaveChangesAsync(cancellationToken);
            ChangeTracker.Clear();
            return operationResult;
        }
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _log.LogInformation("UserDb" + ChangeTracker.Context);
            UpdateOperationsLog(ChangeTracker);
            HandleSoftDelete(ChangeTracker);
            return await base.SaveChangesAsync(cancellationToken);
        }
        private void HandleSoftDelete(ChangeTracker changeTracker)
        {
            foreach (EntityEntry entry in changeTracker.Entries().Where(w => w.State == EntityState.Deleted))
            {
                // Set the entity as Softly Deleted
                entry.Property("IsDeleted").CurrentValue = true;
                // Ensure the entity state is modified to prevend hard deletion
                entry.State = EntityState.Modified;
            }
        }
        private void UpdateOperationsLog(ChangeTracker changeTracker)
        {
            IEnumerable<EntityEntry> changeSet = changeTracker.Entries();
            if (changeSet != null)
            {
                foreach (EntityEntry entry in changeSet.Where(c => c.State != EntityState.Unchanged).ToArray())
                {
                    if (entry.Entity is OperationLogEntity domainBase)
                    {
                        if (entry.State == EntityState.Added && domainBase.IsNew)
                        {
                            ((OperationLogEntity)entry.Entity).CreatedOn = DateTime.UtcNow;
                            ((OperationLogEntity)entry.Entity).CreatedBy = _sessionManager?.UserId == null ? Guid.Empty : _sessionManager.UserId;
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            ((OperationLogEntity)entry.Entity).UpdatedOn = DateTime.UtcNow;
                            ((OperationLogEntity)entry.Entity).UpdatedBy = _sessionManager?.UserId == null ? Guid.Empty : _sessionManager.UserId;
                            Entry((OperationLogEntity)entry.Entity).Property(p => p.CreatedBy).IsModified = false;
                            Entry((OperationLogEntity)entry.Entity).Property(p => p.CreatedOn).IsModified = false;
                        }
                    }
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SchemaName);
            // Addd the Postgres Extension for UUID generation
            //modelBuilder.HasPostgresExtension("uuid-ossp");

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
                {
                    // Discriminator column
                    modelBuilder.Entity(entityType.ClrType).HasDiscriminator("IsDeleted", typeof(bool)).HasValue(false);

                    // Shadow Property
                    modelBuilder.Entity(entityType.ClrType).Property(typeof(bool), "IsDeleted").IsRequired(true);
                    modelBuilder.Entity(entityType.ClrType).Property(typeof(bool), "IsDeleted").Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);

                    // Add IsDelete query filter
                    var parameter = Expression.Parameter(entityType.ClrType);
                    var body = Expression.Not(Expression.Property(parameter, "IsDeleted"));

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, parameter));

                }
                else if (typeof(OperationLogEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Discriminator column
                    modelBuilder.Entity(entityType.ClrType).HasDiscriminator("CreatedBy", typeof(long));
                    modelBuilder.Entity(entityType.ClrType).HasDiscriminator("CreatedOn", typeof(DateTime));
                    modelBuilder.Entity(entityType.ClrType).HasDiscriminator("UpdatedBy", typeof(long?));
                    modelBuilder.Entity(entityType.ClrType).HasDiscriminator("UpdatedOn", typeof(DateTime?));

                    modelBuilder.Entity(entityType.ClrType).Property(typeof(bool), "CreatedBy").Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
                    modelBuilder.Entity(entityType.ClrType).Property(typeof(bool), "CreatedOn").Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
                    modelBuilder.Entity(entityType.ClrType).Property(typeof(bool), "UpdatedBy").Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
                    modelBuilder.Entity(entityType.ClrType).Property(typeof(bool), "UpdatedOn").Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);


                }
            }
        }
    }
}
