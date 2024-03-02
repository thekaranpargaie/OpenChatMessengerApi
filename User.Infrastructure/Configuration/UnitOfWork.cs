using Base.Contracts;
using Base.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace User.Infrastructure.Configuration
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly UserDb _context;
        private readonly IDebugLogger _log;
        public UnitOfWork(
            UserDb context,
            IDebugLogger log)
        {
            this._context = context;
            this._log = log;
        }

        public async Task StartTransactionAsync(CancellationToken cancellationToken = default)
        {
            _log.LogInformation("User Context State: " + _context.Database.CurrentTransaction);
            await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> CommitTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _context.SaveChangesAsync(cancellationToken);
            await _context.Database.CommitTransactionAsync();
            return result;
        }
        public async Task RollBackTransactionAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _log.LogInformation("User Context State: " + _context.Database.CurrentTransaction);
            await _context.Database.RollbackTransactionAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Cleanup();
        }
        public void Cleanup()
        {
            if(_context is not null && _context.Database is not null)
            {
                _context.Database.CloseConnection();
            }
        }
    }
}
