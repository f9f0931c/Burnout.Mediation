using System;
using System.Collections.Generic;
using System.Threading;

namespace Burnout.Mediation;

public interface IDispatcher<TResult>
{
	IAsyncEnumerable<TResult> Dispatch(IServiceProvider services, object input, CancellationToken cancellationToken);
}
