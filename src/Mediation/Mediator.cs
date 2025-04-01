using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation
{
    public class Mediator : IMediator
    {
        readonly IServiceProvider Services;

        public Mediator(
            IServiceProvider services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public TReturn Dispatch<TReturn>(
            Func<IServiceProvider, IDispatcher<TReturn>> factory,
            object input,
            CancellationToken cancellationToken)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            _ = input ?? throw new ArgumentNullException(nameof(input));
            var dispatcher = factory(Services);
            cancellationToken.ThrowIfCancellationRequested();
            return dispatcher.Dispatch(Services, input, cancellationToken);
        }
    }
}
