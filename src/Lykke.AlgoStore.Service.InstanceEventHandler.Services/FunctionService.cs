using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Repositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class FunctionService : IFunctionService
    {
        private readonly IHandler<FunctionChartingUpdate> _functionHandler;
        private readonly IFunctionChartingUpdateRepository _functionChartingUpdateRepository;

        public FunctionService(IHandler<FunctionChartingUpdate> functionHandler,
            IFunctionChartingUpdateRepository functionChartingUpdateRepository)
        {
            _functionHandler = functionHandler;
            _functionChartingUpdateRepository = functionChartingUpdateRepository;
        }

        public async Task WriteAsync(IEnumerable<FunctionChartingUpdate> functions)
        {
            var functionChartingUpdates = functions.ToList();

            //Store function values
            await _functionChartingUpdateRepository.WriteAsync(functionChartingUpdates);

            //Emit function values
            foreach (var function in functionChartingUpdates)
            {
                await _functionHandler.Handle(Mapper.Map<FunctionChartingUpdate>(function));
            }
        }
    }
}
