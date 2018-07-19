using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services.Handlers
{
    public class TradeHandler : IHandler<TradeChartingUpdate>
    {
        private readonly IHandler<TradeChartingUpdate> _rabbitMqHandler;

        public TradeHandler(IHandler<TradeChartingUpdate> rabbitMqHandler)
        {
            _rabbitMqHandler = rabbitMqHandler;
        }

        public async Task Handle(TradeChartingUpdate message)
        {
            await _rabbitMqHandler.Handle(message);
        }
    }
}
