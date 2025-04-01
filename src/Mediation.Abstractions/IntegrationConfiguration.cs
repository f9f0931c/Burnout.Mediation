using System;
using System.Collections.Generic;
using System.Text;

namespace Burnout.Mediation;

public class IntegrationConfiguration
{
    public IntegrationDefinition Definition { get; }

    public IntegrationConfiguration(
        IntegrationDefinition definition)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
    }
}
