using Burnout.Mediation.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation;

public static class MediatorExtensions
{
    static readonly Type _definition = typeof(NotificationDispatcher<>);

	static async Task PublishAsync(
		this IMediator mediator,
		IDispatcher<byte> dispatcher,
		object message,
		CancellationToken cancellationToken = default)
	{
		var enumerator = mediator
			.Dispatch(dispatcher, message, cancellationToken)
			.GetAsyncEnumerator();

		// If there's a result then something has gone wrong
		if (await enumerator.MoveNextAsync())
			throw new Exception();
	}

	public static Task PublishAsync(
		this IMediator mediator,
		object message,
		CancellationToken cancellationToken = default)
	{
		var dispatcher = (IDispatcher<byte>)
			Activator.CreateInstance(
				_definition.MakeGenericType(message.GetType()));

		return mediator.PublishAsync(dispatcher, message, cancellationToken);
	}

	public static Task PublishAsync<TMessage>(
		this IMediator mediator,
		TMessage message,
		CancellationToken cancellationToken = default)
	{
		var dispatcher = new NotificationDispatcher<TMessage>();
		return mediator.PublishAsync(dispatcher, message, cancellationToken);
	}


    public static IMediator AddPipelines(
        this IMediator mediator)
    {
        return new PipelineMediator(mediator);
    }
}

class PipelineMediator : IMediator
{
    readonly IMediator Mediator;

    public PipelineMediator(
        IMediator mediator)
    {
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public IAsyncEnumerable<TRecord> Dispatch<TRecord>(Func<IServiceProvider, IDispatcher<TRecord>> factory, object input, CancellationToken cancellationToken)
    {
		return Mediator.Dispatch(
            new PipelineDispatcher<TRecord>(factory), 
            input, 
            cancellationToken);
    }
}
