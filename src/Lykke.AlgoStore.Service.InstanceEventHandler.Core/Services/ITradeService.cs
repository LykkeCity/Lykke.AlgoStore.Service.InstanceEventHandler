using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services
{
    public interface ITradeService
    {
        Task WriteAsync(AlgoClientInstanceData clientInstanceData, IEnumerable<TradeChartingUpdate> trades);
    }
}
