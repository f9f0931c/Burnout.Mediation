using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Burnout.Mediation;

public class BasicMediationRegistrar : IMediationRegistrar<IServiceCollection>
{
    public void Register(IServiceCollection services, MediatorConfiguration configuration)
    {
        var concretions = configuration.Types.Where(x => x.IsConcrete() && !x.IsOpenGeneric());
        foreach (var concretion in concretions)
        {
            foreach (var search in configuration.Integrations)
            {
                var scanned = concretion
                    .GetMatchingGenericTypeDefinitions(search.Definition.Type)
                    .Where(x => x.IsInterface)
                    .Where(x => search.Definition.AllowMultiple || concretion.MatchesInterfaceTypeArguments(x));
                foreach (var entry in scanned)
                {
                    if (search.Definition.AllowMultiple) services.AddTransient(entry, concretion);
                    else services.TryAddTransient(entry, concretion);
                }
            }
        }
    }
}
