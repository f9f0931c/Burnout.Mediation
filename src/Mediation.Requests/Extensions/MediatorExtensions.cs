using Burnout.Mediation.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation;

public static class MediatorExtensions
{
    static readonly Type _sendDefinition = typeof(RequestDispatcher<>);
	static readonly Type _requestDefinition = typeof(RequestDispatcher<,>);

	static async Task SendAsync<TRequest>(
		this IMediator mediator,
		IDispatcher2<byte> dispatcher,
		TRequest request,
		CancellationToken cancellationToken = default)
	{
		var enumerator = mediator
			.Dispatch(dispatcher, request, cancellationToken)
			.GetAsyncEnumerator();

		// Shouldn't be receiving any response
		if (await enumerator.MoveNextAsync())
			throw new Exception();
	}

	public static Task SendAsync(
		this IMediator mediator,
		object request,
		CancellationToken cancellationToken = default)
	{
		var dispatcher = (IDispatcher2<byte>)
			Activator.CreateInstance(
				_sendDefinition.MakeGenericType(
					request.GetType()));

		return mediator.SendAsync(dispatcher, request, cancellationToken);
	}

    public static Task SendAsync<TRequest>(
        this IMediator mediator,
        TRequest request,
        CancellationToken cancellationToken = default)
	{
		var dispatcher = new RequestDispatcher<TRequest>();
		return mediator.RequestAsync(dispatcher, request, cancellationToken);
	}

	static async Task<TResult> RequestAsync<TRequest, TResult>(
		this IMediator mediator,
		IDispatcher2<TResult> dispatcher,
		TRequest request,
		CancellationToken cancellationToken = default)
	{
		var enumerator = mediator
			.Dispatch(dispatcher, request, cancellationToken)
			.GetAsyncEnumerator();

		// Should have a return value
		var result = await enumerator.MoveNextAsync() ? enumerator.Current :
			throw new Exception();

		// Shouldn't be receiving multiple values
		if (await enumerator.MoveNextAsync())
			throw new Exception();

		return result;
	}

	public static Task<TResult> RequestAsync<TResult>(
        this IMediator mediator,
        object request,
        CancellationToken cancellationToken = default)
	{
		var dispatcher = (IDispatcher2<TResult>)
			Activator.CreateInstance(
				_requestDefinition.MakeGenericType(
					request.GetType(), typeof(TResult)));

		return mediator.RequestAsync(dispatcher, request, cancellationToken);
	}

    public static Task<TResult> RequestAsync<TRequest, TResult>(
        this IMediator mediator,
        TRequest request,
        CancellationToken cancellationToken = default)
	{
		var dispatcher = new RequestDispatcher<TRequest, TResult>();
		return mediator.RequestAsync(dispatcher, request, cancellationToken);
	}
}
