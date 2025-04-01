using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Burnout.Mediation.Requests;

namespace Burnout.Mediation;

public static class MediationBuilderExtensions
{
    public static IMediationBuilder AddRequests(
        this IMediationBuilder builder,
        Action<IIntegrationBuilder> configuration)
    {
        return builder.AddIntegration(
            new IntegrationDefinition(
                typeof(IRequestHandler<,>),
                false),
            configuration);
    }
}
