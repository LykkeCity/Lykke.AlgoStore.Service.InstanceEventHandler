using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Domain;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class TradeService : ITradeService
    {
        private readonly IHandler<Trade> _tradeHandler;

        public TradeService(IHandler<Trade> tradeHandler)
        {
            _tradeHandler = tradeHandler;
        }

        public async Task WriteAsync(IEnumerable<ITrade> trades)
        {
            foreach (var trade in trades)
            {
                await _tradeHandler.Handle(Mapper.Map<Trade>(trade));
            }
        }
    }
}
