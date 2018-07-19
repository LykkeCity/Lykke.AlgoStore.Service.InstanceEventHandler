using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class FunctionService : IFunctionService
    {
        private readonly IHandler<FunctionChartingUpdate> _functionHandler;

        public FunctionService(IHandler<FunctionChartingUpdate> functionHandler)
        {
            _functionHandler = functionHandler;
        }

        public async Task WriteAsync(IEnumerable<FunctionChartingUpdate> functions)
        {
            foreach (var function in functions)
            {
                await _functionHandler.Handle(Mapper.Map<FunctionChartingUpdate>(function));
            }
        }
    }
}
