using Burnout.Mediation.Notifications;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation;

public static class MediatorExtensions
{
    static readonly Type _definition = typeof(NotificationDispatcher<>);

    public static Task SendAsync(
        this IMediator mediator,
        object message,
        CancellationToken cancellationToken = default) =>
        mediator.Dispatch(
            (IDispatcher<Task>)
            Activator.CreateInstance(
                _definition
                    .MakeGenericType(message.GetType())),
            message,
            cancellationToken);

    public static Task SendAsync<TMessage>(
        this IMediator mediator,
        TMessage message,
        CancellationToken cancellationToken = default) =>
        mediator.Dispatch(
            new NotificationDispatcher<TMessage>(),
            message,
            cancellationToken);

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

    public TReturn Dispatch<TReturn>(Func<IServiceProvider, IDispatcher<TReturn>> factory, object input, CancellationToken cancellationToken)
    {
        return Mediator.Dispatch(
            new PipelineDispatcher<TReturn>(factory), 
            input, 
            cancellationToken);
    }
}
