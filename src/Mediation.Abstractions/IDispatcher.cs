using System;
using System.Collections.Generic;
using System.Threading;

namespace Burnout.Mediation;

public interface IDispatcher2<TResult>
{
	IAsyncEnumerable<TResult> Dispatch(IServiceProvider services, object input, CancellationToken cancellationToken);
}
