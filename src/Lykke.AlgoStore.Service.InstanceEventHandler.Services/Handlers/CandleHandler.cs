using System.Threading.Tasks;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services.Handlers
{
    public class CandleHandler : IHandler<Candle>
    {
        private readonly IHandler<Candle> _rabbitMqHandler;

        public CandleHandler(IHandler<Candle> rabbitMqHandler)
        {
            _rabbitMqHandler = rabbitMqHandler;
        }

        public async Task Handle(Candle message)
        {
            await _rabbitMqHandler.Handle(message);
        }
    }
}
