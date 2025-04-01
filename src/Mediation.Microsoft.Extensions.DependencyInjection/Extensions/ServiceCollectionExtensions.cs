using Burnout.Mediation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Burnout.Mediation.Microsoft.Extensions.DependencyInjection.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
 
public static class ServiceCollectionExtensions {
    public static IServiceCollection AddMediatorConfiguration(
        this IServiceCollection services,
        IMediationRegistrar<IServiceCollection> registrar,
        Action<IMediationBuilder> configuration)
    {
        var builder = new MediationBuilder();
        configuration(builder);
        registrar.Register(services, builder.Build());
        return services;
    }
}
