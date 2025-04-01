using System.Threading;
using System.Threading.Tasks;

namespace Burnout.Mediation.Notifications; 

public interface INotificationHandler<in TMessage> 
{
    Task HandleAsync(TMessage request, CancellationToken cancellationToken);
}
