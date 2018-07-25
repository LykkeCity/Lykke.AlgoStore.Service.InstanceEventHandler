using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Core.Repositories
{
    public interface IFunctionChartingUpdateRepository
    {
        Task WriteAsync(IEnumerable<FunctionChartingUpdate> data);
    }
}
