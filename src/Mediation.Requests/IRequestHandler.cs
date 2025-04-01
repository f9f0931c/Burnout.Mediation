using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation.Requests; 

public interface IRequestHandler<in TRequest, TResult>
{
    Task<TResult> HandleAsync(TRequest input, CancellationToken cancellationToken);
}
