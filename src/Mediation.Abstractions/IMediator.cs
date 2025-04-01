using System;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation;

public interface IMediator 
{
    TReturn Dispatch<TReturn>(
        Func<IServiceProvider, IDispatcher<TReturn>> factory,
        object input,
        CancellationToken cancellationToken);
}
