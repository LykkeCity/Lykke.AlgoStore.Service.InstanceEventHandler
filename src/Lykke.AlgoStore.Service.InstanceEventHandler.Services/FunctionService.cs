using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Repositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Utils;

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

            ValidateFunctionChartingUpdateData(functionChartingUpdates);

            //Store function values
            await _functionChartingUpdateRepository.WriteAsync(functionChartingUpdates);

            //Emit function values
            foreach (var function in functionChartingUpdates)
            {
                await _functionHandler.Handle(Mapper.Map<FunctionChartingUpdate>(function));
            }
        }

        private static void ValidateFunctionChartingUpdateData(List<FunctionChartingUpdate> functionChartingUpdateData)
        {
            if(functionChartingUpdateData == null)
                throw new ArgumentNullException(nameof(functionChartingUpdateData));

            if(!functionChartingUpdateData.Any())
                throw new ValidationException(Phrases.FunctionValuesCannotBeEmpty);

            if (functionChartingUpdateData.Count > 100)
                throw new ValidationException(Phrases.MaxRecordsPerBatchReached);

            var flattenedData = functionChartingUpdateData.Flatten(x => x.InnerFunctions).ToList();

            if(flattenedData.Select(x => x.InstanceId).Distinct().Count() > 1)
                throw new ValidationException(Phrases.SameInstanceIdForAllFunctionValues);

            if(flattenedData.Any(x => string.IsNullOrEmpty(x.InstanceId)))
                throw new ValidationException(Phrases.InstanceIdForAllFunctionValues);

            if(flattenedData.Any(x => string.IsNullOrEmpty(x.FunctionName)))
                throw new ValidationException(Phrases.AllFunctionNamesMustBeProvided);
        }
    }
}
