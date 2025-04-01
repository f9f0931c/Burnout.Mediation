using System;
using System.Collections.Generic;
using System.Text;

namespace Burnout.Mediation;

public interface IMediationRegistrar<in TContainer>
{
    void Register(TContainer services, MediatorConfiguration configuration);
}
