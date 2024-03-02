using Base.Contracts;

namespace User.Infrastructure.Configuration
{
    public class UnitOfWorkCommandHandlerWithResultDecorator<T, TResult> : ICommandHandler<T, TResult> where T : ICommand<TResult>
    {
        private readonly ICommandHandler<T, TResult> _decorated;
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkCommandHandlerWithResultDecorator(
            ICommandHandler<T, TResult> decorated,
            IUnitOfWork unitOfWork)
        {
            _decorated = decorated;
            _unitOfWork = unitOfWork;
        }

        public async Task<TResult> Handle(T request, CancellationToken cancellationToken)
        {

            using (this._unitOfWork.StartTransactionAsync())
            {
                try
                {
                    var result = await this._decorated.Handle(request, cancellationToken);
                    await this._unitOfWork.CommitTransactionAsync(cancellationToken);
                    return result;
                }
                catch(Exception ex) 
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken);
                    throw new Exception(ex.ToString());
                }
            }



        }
    }
}
