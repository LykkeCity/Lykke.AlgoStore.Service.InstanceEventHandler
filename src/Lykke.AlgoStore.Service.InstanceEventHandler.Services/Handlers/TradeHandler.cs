using System.Threading.Tasks;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services.Handlers
{
    public class TradeHandler : IHandler<Trade>
    {
        private readonly IHandler<Trade> _rabbitMqHandler;

        public TradeHandler(IHandler<Trade> rabbitMqHandler)
        {
            _rabbitMqHandler = rabbitMqHandler;
        }

        public async Task Handle(Trade message)
        {
            await _rabbitMqHandler.Handle(message);
        }
    }
}
