using Base.Contracts;
using MediatR;

namespace User.Infrastructure.Configuration
{
    public class UnitOfWorkCommandHandlerDecorator<T> : ICommandHandler<T> where T : ICommand
    {
        private readonly ICommandHandler<T> _decorated;
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkCommandHandlerDecorator(
            ICommandHandler<T> decorated,
            IUnitOfWork unitOfWork)
        {
            _decorated = decorated;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(T request, CancellationToken cancellationToken)
        {
            using (this._unitOfWork.StartTransactionAsync())
            {
                try
                {
                    await this._decorated.Handle(request, cancellationToken);
                    await this._unitOfWork.CommitTransactionAsync(cancellationToken);
                    return Unit.Value;
                }
                catch(Exception ex)
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken);
                    throw;
                }
            }
       

          

        }
    }
}
