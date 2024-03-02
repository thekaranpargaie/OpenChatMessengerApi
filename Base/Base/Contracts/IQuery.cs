using MediatR;

namespace Base.Contracts
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {

    }
}
