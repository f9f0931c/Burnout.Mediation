using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation.Notifications;

class NotificationDispatcher<TMessage> : IDispatcher2<byte>
{
    public async IAsyncEnumerable<byte> Dispatch(IServiceProvider services, object input, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
		var message = (TMessage)input;
		var handlers = (IEnumerable<INotificationHandler<TMessage>>)services
			.GetService(typeof(IEnumerable<INotificationHandler<TMessage>>));

		foreach (var handler in handlers ?? [])
			await handler
				.HandleAsync(message, cancellationToken)
				.ConfigureAwait(false);
		yield break;
    }
}

public interface IPipelineStepFactory<TReturn>
{
    IPipelineStep<TReturn> Create();
}

public interface IPipelineStep<TRecord>
{
    IAsyncEnumerable<TRecord> Handle(object input, Func<object, CancellationToken, IAsyncEnumerable<TRecord>> next, CancellationToken cancellationToken);
}

public delegate IAsyncEnumerable<TRecord> PipelineStepHandle<TRecord>(object input, CancellationToken cancellationToken);

public interface IPipelineStep2<TRecord>
{
	IAsyncEnumerable<TRecord> Handle(object input, PipelineStepHandle<TRecord> next, CancellationToken cancellationToken);
}

class LoggingStep<T> : IPipelineStep2<T>
{
    public async IAsyncEnumerable<T> Handle(object input, PipelineStepHandle<T> next, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
		// Log something here for entry
		await foreach (var record in next(input, cancellationToken))
			// Log something here for each record (streams/requests)
			yield return record;

		// Log something here at exit
    }
}

class PipelineDispatcher<T> : IDispatcher2<T>
{
    readonly Func<IServiceProvider, IDispatcher2<T>> _factory;

    public PipelineDispatcher(
        Func<IServiceProvider, IDispatcher2<T>> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public IAsyncEnumerable<T> Dispatch(IServiceProvider serviceProvider, object input, CancellationToken cancellationToken)
    {
        var steps = (IEnumerable<IPipelineStep<T>>)serviceProvider.GetService(typeof(IEnumerable<IPipelineStep<T>>));
        var handler = _factory(serviceProvider);
        var pipeline = steps.Reverse().Aggregate(
            (object value, CancellationToken cancellation) => handler.Dispatch(serviceProvider, input, cancellation),
            (next, step) => (value, cancellation) => step.Handle(value, next, cancellation));
        return pipeline(input, cancellationToken);
    }
}
