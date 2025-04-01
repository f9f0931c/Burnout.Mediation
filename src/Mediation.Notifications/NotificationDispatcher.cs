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
