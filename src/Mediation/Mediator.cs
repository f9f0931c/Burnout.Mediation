using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation
{
    public class Mediator : IMediator
    {
        readonly IServiceProvider _services;

        public Mediator(
            IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

		public IAsyncEnumerable<TRecord> Dispatch<TRecord>(
			Func<IServiceProvider, IDispatcher<TRecord>> factory,
			object input,
			CancellationToken cancellationToken)
		{
		    _ = factory ?? throw new ArgumentNullException(nameof(factory));
            _ = input ?? throw new ArgumentNullException(nameof(input));
 			var dispatcher = factory(_services);
			cancellationToken.ThrowIfCancellationRequested();
			return dispatcher.Dispatch(_services, input, cancellationToken);
		}
    }
}
