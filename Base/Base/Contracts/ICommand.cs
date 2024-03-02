using MediatR;

namespace Base.Contracts
{
    public interface ICommand<out TResult> : IRequest<TResult>
    {
        Guid Id { get; }
    }

    public interface ICommand : IRequest
    {
        Guid Id { get; }
    }

    public interface IQuery : IRequest
    {
        Guid Id { get; }
    }
}
