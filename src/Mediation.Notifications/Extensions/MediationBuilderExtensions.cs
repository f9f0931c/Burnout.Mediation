using System;
using System.Collections.Generic;
using System.Text;
using Burnout.Mediation.Notifications;

namespace Burnout.Mediation;

public static class MediationBuilderExtensions
{
    public static IMediationBuilder AddNotifications(
        this IMediationBuilder builder,
        Action<IIntegrationBuilder> configuration)
    {
        return builder.AddIntegration(
            new IntegrationDefinition(
                typeof(INotificationHandler<>),
                true),
            configuration);
    }
}
