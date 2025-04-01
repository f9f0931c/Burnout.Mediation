using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Burnout.Mediation.Microsoft.Extensions.DependencyInjection.Configuration; 

public class MediatorServiceConfiguration<TMediator>
    where TMediator : class, IMediator {
    internal Type Implementation { get; } = typeof(TMediator);

    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
}
