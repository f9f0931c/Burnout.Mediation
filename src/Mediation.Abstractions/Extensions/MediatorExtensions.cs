using System;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation;
 
public static class MediatorExtensions {
    public static TReturn Dispatch<TReturn>(
        this IMediator mediator,
        IDispatcher<TReturn> dispatcher,
        object input,
        CancellationToken cancellationToken) =>
        mediator.Dispatch(
            services => dispatcher,
            input,
            cancellationToken);
}
