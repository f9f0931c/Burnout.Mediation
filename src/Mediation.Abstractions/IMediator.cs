using System;
using System.Collections.Generic;
using System.Threading;

namespace Burnout.Mediation;

public interface IMediator 
{
	IAsyncEnumerable<TRecord> Dispatch<TRecord>(
		Func<IServiceProvider, IDispatcher<TRecord>> factory,
		object input,
		CancellationToken cancellationToken);
}
