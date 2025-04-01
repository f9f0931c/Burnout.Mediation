using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation.Requests; 

internal class RequestDispatcher<TRequest, TResult> : IDispatcher<Task<TResult>> 
{        
    public Task<TResult> Dispatch(
        IServiceProvider services, 
        object input, 
        CancellationToken cancellationToken) 
    {
        return services
            .GetRequiredService<IRequestHandler<TRequest, TResult>>()
            .HandleAsync((TRequest)input, cancellationToken);
    }
}
