using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation;

public interface IDispatcher<out TReturn>
{
    TReturn Dispatch(IServiceProvider serviceProvider, object input, CancellationToken cancellationToken);
}
