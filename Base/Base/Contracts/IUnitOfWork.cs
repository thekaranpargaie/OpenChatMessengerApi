namespace Base.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> CommitTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task StartTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task RollBackTransactionAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
