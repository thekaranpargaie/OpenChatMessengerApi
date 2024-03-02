using Base.Contracts;
using MediatR;

namespace User.Infrastructure
{
    public interface IUserModule : IModule
    {

    }
    public class UserModule : IUserModule
    {
        private readonly IMediator _mediator;
        public UserModule(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command)
        {
            return await _mediator.Send(command);
        }

        public async Task ExecuteCommandAsync(ICommand command)
        {
            await _mediator.Send(command);
        }

        public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
        {
            return await _mediator.Send(query);
        }
    }
}
