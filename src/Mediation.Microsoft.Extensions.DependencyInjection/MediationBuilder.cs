using Burnout.Mediation.Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burnout.Mediation;

class MediationBuilder : IMediationBuilder
{
    readonly LinkedList<IntegrationConfiguration> Integrations =
        new LinkedList<IntegrationConfiguration>();

    readonly ISet<Type> Types = new HashSet<Type>();

    public IMediationBuilder AddTypes(IEnumerable<Type> types)
    {
        _ = types ?? throw new ArgumentNullException(nameof(types));
        foreach (var type in types)
            Types.Add(type);
        return this;
    }

    public IMediationBuilder AddIntegration(IntegrationDefinition definition, Action<IIntegrationBuilder>? configuration)
    {
        _ = definition ?? throw new ArgumentNullException(nameof(definition));
        Integrations.AddLast(
            new IntegrationConfiguration(
                definition));
        return this;
    }

    internal MediatorConfiguration Build()
    {
        return new MediatorConfiguration(
            Types.ToArray(),
            Integrations.ToArray());
    }
}
