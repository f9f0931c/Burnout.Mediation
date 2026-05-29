using System;
using System.Collections.Generic;
using System.Threading;

namespace Burnout.Mediation.Streaming;

class StreamDispatcher<TRequest, TRecord> : IDispatcher2<TRecord>
{
    public IAsyncEnumerable<TRecord> Dispatch(
        IServiceProvider services,
        object input,
        CancellationToken cancellationToken)
    {
		var request = (TRequest)input;
		var handler = (IStreamRequestHandler<TRequest, TRecord>)services
			.GetService(typeof(IStreamRequestHandler<TRequest, TRecord>));

		return handler.StreamAsync(request, cancellationToken);
    }
}
