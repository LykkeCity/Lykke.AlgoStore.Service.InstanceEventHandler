using System.Threading.Tasks;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services.Handlers
{
    public class FunctionHandler : IHandler<Function>
    {
        private readonly IHandler<Function> _rabbitMqHandler;

        public FunctionHandler(IHandler<Function> rabbitMqHandler)
        {
            _rabbitMqHandler = rabbitMqHandler;
        }

        public async Task Handle(Function message)
        {
            await _rabbitMqHandler.Handle(message);
        }
    }
}
