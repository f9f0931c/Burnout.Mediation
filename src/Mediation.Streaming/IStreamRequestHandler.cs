using System;
using System.Collections.Generic;
using System.Threading;

namespace Burnout.Mediation.Streaming;

interface IStreamRequestHandler<TRequest, TRecord>
{
    IAsyncEnumerable<TRecord> StreamAsync(TRequest request, CancellationToken cancellationToken);
}
