using Burnout.Mediation.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation;

public static class MediatorExtensions
{
    static readonly Type _definition = typeof(RequestDispatcher<,>);

    public static Task<TResult> RequestAsync<TResult>(
        this IMediator mediator,
        object request,
        CancellationToken cancellationToken = default) =>
        mediator.Dispatch(
            (IDispatcher<Task<TResult>>)
                Activator.CreateInstance(
                    _definition
                        .MakeGenericType(
                            request.GetType(),
                            typeof(TResult))),
            request,
            cancellationToken);

    public static Task<TResult> RequestAsync<TRequest, TResult>(
        this IMediator mediator,
        TRequest request,
        CancellationToken cancellationToken = default) => 
        mediator.Dispatch(
            new RequestDispatcher<TRequest, TResult>(),
            request, 
            cancellationToken);
}
