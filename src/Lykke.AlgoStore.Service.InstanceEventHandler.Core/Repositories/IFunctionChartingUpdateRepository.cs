using Lykke.AlgoStore.Algo.Charting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Core.Repositories
{
    public interface IFunctionChartingUpdateRepository
    {
        Task WriteAsync(IEnumerable<FunctionChartingUpdate> data);
        Task SaveDifferentPartionsAsync(IEnumerable<FunctionChartingUpdate> data);
    }
}
