using System;
using System.Collections.Generic;
using System.Text;

namespace Burnout.Mediation;

public interface IMediationBuilder
{
    IMediationBuilder AddTypes(
        IEnumerable<Type> types);

    IMediationBuilder AddIntegration(
        IntegrationDefinition definition,
        Action<IIntegrationBuilder>? configuration);
}
