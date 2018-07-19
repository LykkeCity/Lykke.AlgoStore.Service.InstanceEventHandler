using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services
{
    public interface ITradeService
    {
        Task WriteAsync(IEnumerable<TradeChartingUpdate> trades);
    }
}
