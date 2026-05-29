using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation.Requests; 

class RequestDispatcher<TRequest> : IDispatcher2<byte>
{
    public async IAsyncEnumerable<byte> Dispatch(IServiceProvider services, object input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
		var handler = (IRequestHandler<TRequest>)services
			.GetService(typeof(IRequestHandler<TRequest>));

		await handler.HandleAsync((TRequest)input, cancellationToken);

		yield break;
    }
}

class RequestDispatcher<TRequest, TResult> : IDispatcher2<TResult>
{
    public async IAsyncEnumerable<TResult> Dispatch(IServiceProvider services, object input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
		var handler = (IRequestHandler<TRequest, TResult>)services
			.GetService(typeof(IRequestHandler<TRequest, TResult>));

		var result = await handler
			.HandleAsync((TRequest)input, cancellationToken);

		yield return result;
    }
}
