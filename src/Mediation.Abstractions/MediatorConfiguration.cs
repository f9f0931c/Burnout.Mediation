using System;
using System.Collections.Generic;
using System.Text;

namespace Burnout.Mediation;

public class MediatorConfiguration
{
    public IReadOnlyCollection<Type> Types { get; }
    public IReadOnlyCollection<IntegrationConfiguration> Integrations { get; }

    public MediatorConfiguration(
        IReadOnlyCollection<Type> types,
        IReadOnlyCollection<IntegrationConfiguration> integrations)
    {
        Types = types ?? throw new ArgumentNullException(nameof(types));
        Integrations = integrations ?? throw new ArgumentNullException(nameof(integrations));
    }
}
