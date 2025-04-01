using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation.Notifications;

class NotificationDispatcher<TMessage> : IDispatcher<Task>
{
    public async Task Dispatch(
        IServiceProvider services,
        object input,
        CancellationToken cancellationToken)
    {
        var message = (TMessage)input;
        foreach (var handler in services.GetServices<INotificationHandler<TMessage>>())
            await handler
                .HandleAsync(message, cancellationToken)
                .ConfigureAwait(false);
    }
}

interface IStreamRequestHandler<TRequest, TRecord>
{
    IAsyncEnumerable<TRecord> StreamAsync(TRequest request, CancellationToken cancellationToken);
}

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

public interface IPipelineStepFactory<TReturn>
{
    IPipelineStep<TReturn> Create();
}

public interface IPipelineStep<TReturn>
{
    TReturn Handle(object input, Func<object, CancellationToken, TReturn> next, CancellationToken cancellationToken);
}

class NotificationLoggingStep<T> : IPipelineStep<Task>
{
    public async Task Handle(object input, Func<object, CancellationToken, Task> next, CancellationToken cancellationToken)
    {
        await next(input, cancellationToken);
    }
}

class ValueLoggingStep<T> : IPipelineStep<Task<T>>
{
    public async Task<T> Handle(object input, Func<object, CancellationToken, Task<T>> next, CancellationToken cancellationToken)
    {
        return await next(input, cancellationToken);
    }
}

class StreamLoggingStep<T> : IPipelineStep<IAsyncEnumerable<T>>
{
    public async IAsyncEnumerable<T> Handle(object input, Func<object, CancellationToken, IAsyncEnumerable<T>> next, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var record in next(input, cancellationToken))
            yield return record;
    }
}

class PipelineDispatcher<T> : IDispatcher<T>
{
    readonly Func<IServiceProvider, IDispatcher<T>> _factory;

    public PipelineDispatcher(
        Func<IServiceProvider, IDispatcher<T>> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public T Dispatch(IServiceProvider serviceProvider, object input, CancellationToken cancellationToken)
    {
        var steps = serviceProvider.GetServices<IPipelineStep<T>>();
        var handler = _factory(serviceProvider);
        var pipeline = steps.Reverse().Aggregate(
            (object value, CancellationToken cancellation) => handler.Dispatch(serviceProvider, input, cancellation),
            (next, step) => (value, cancellation) => step.Handle(value, next, cancellation));
        return pipeline(input, cancellationToken);
    }
}
