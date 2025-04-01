using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Burnout.Mediation;

public interface IDispatcherProvider<TReturn>
{
    IDispatcher<TReturn> Create(IServiceProvider services);
}
