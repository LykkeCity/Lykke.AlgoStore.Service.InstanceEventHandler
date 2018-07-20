using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class CandleService : ICandleService
    {
        private readonly IHandler<CandleChartingUpdate> _candleHandler;

        public CandleService(IHandler<CandleChartingUpdate> candleHandler)
        {
            _candleHandler = candleHandler;
        }

        public async Task WriteAsync(IEnumerable<CandleChartingUpdate> candles)
        {
            foreach (var candle in candles)
            {
                await _candleHandler.Handle(candle);
            }
        }
    }
}
