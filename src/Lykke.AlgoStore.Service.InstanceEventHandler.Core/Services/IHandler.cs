using System.Threading.Tasks;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services
{
    public interface IHandler<in T>
    {
        Task Handle(T message);
    }
}
