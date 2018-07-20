using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class TradeService : ITradeService
    {
        private readonly IHandler<TradeChartingUpdate> _tradeHandler;

        public TradeService(IHandler<TradeChartingUpdate> tradeHandler)
        {
            _tradeHandler = tradeHandler;
        }

        public async Task WriteAsync(IEnumerable<TradeChartingUpdate> trades)
        {
            foreach (var trade in trades)
            {
                await _tradeHandler.Handle(trade);
            }
        }
    }
}
