using System;
using System.Collections.Generic;
using System.Text;

namespace Burnout.Mediation;

public class IntegrationDefinition
{
    public Type Type { get; }
    public bool AllowMultiple { get; }

    public IntegrationDefinition(
        Type type,
        bool allowMultiple)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type)); ;
        AllowMultiple = allowMultiple;
    }
}
