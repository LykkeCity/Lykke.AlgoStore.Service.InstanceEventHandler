using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Domain;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class CandleService : ICandleService
    {
        private readonly IHandler<Candle> _candleHandler;

        public CandleService(IHandler<Candle> candleHandler)
        {
            _candleHandler = candleHandler;
        }

        public async Task WriteAsync(IEnumerable<ICandle> candles)
        {
            foreach (var candle in candles)
            {
                await _candleHandler.Handle(Mapper.Map<Candle>(candle));
            }
        }
    }
}
