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
}
