using System.Collections.Generic;
using System.Threading;

namespace Burnout.Mediation; 

public static class MediatorExtensions {
	public static IAsyncEnumerable<TRecord> Dispatch<TRecord>(
        this IMediator mediator,
        IDispatcherProvider<TRecord> factory,
        object input,
        CancellationToken cancellationToken) =>
        mediator.Dispatch(
            factory.Create,
            input,
            cancellationToken);

	public static IAsyncEnumerable<TRecord> Dispatch<TRecord>(
		this IMediator mediator,
		IDispatcher<TRecord> dispatcher,
		object input,
		CancellationToken cancellationToken) =>
		mediator.Dispatch(
			services => dispatcher,
			input,
			cancellationToken);
}
