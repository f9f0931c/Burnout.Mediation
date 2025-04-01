using System;
using System.Collections.Generic;
using System.Threading;
using Burnout.Mediation.Streaming;

namespace Burnout.Mediation;

public static class MediatorExtensions
{
    static readonly Type _definition = typeof(StreamDispatcher<,>);
    
    public static IAsyncEnumerable<TResult> StreamAsync<TResult>(
        this IMediator mediator,
        object request,
        CancellationToken cancellationToken = default) =>
        mediator.Dispatch(
            (IDispatcher<IAsyncEnumerable<TResult>>)
            Activator.CreateInstance(
                _definition.MakeGenericType(
                    request.GetType(),
                    typeof(TResult))),
                request,
                cancellationToken);

    public static IAsyncEnumerable<TResult> StreamAsync<TRequest, TResult>(
            this IMediator mediator,
            TRequest request,
            CancellationToken cancellationToken = default) =>
        mediator.Dispatch(
                new StreamDispatcher<TRequest, TResult>(),
                request,
                cancellationToken);
}
