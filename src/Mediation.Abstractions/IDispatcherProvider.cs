using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Burnout.Mediation;

public interface IDispatcherProvider<TRecord>
{
    IDispatcher<TRecord> Create(IServiceProvider services);
}
