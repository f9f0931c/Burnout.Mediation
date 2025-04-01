using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Burnout.Mediation.Streaming;

class StreamDispatcher<TRequest, TRecord> : IDispatcher<IAsyncEnumerable<TRecord>>
{
    public IAsyncEnumerable<TRecord> Dispatch(
        IServiceProvider services,
        object input,
        CancellationToken cancellationToken)
    {
        var handler = services.GetRequiredService<IStreamRequestHandler<TRequest, TRecord>>();
        return handler.StreamAsync((TRequest)input, cancellationToken);
    }
}
