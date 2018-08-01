using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Repositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Utils;
using Lykke.Common.Log;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class FunctionService : IFunctionService
    {
        private readonly IHandler<FunctionChartingUpdate> _functionHandler;
        private readonly IFunctionChartingUpdateRepository _functionChartingUpdateRepository;
        private readonly ILog _log;

        public FunctionService(IHandler<FunctionChartingUpdate> functionHandler,
            IFunctionChartingUpdateRepository functionChartingUpdateRepository, 
            ILogFactory logFactory)
        {
            _functionHandler = functionHandler;
            _functionChartingUpdateRepository = functionChartingUpdateRepository;
            _log = logFactory.CreateLog(this);
        }

        public async Task WriteAsync(IEnumerable<FunctionChartingUpdate> functions)
        {
            var functionChartingUpdates = functions.ToList();

            var functionsDetails = JsonConvert.SerializeObject(functionChartingUpdates);

            _log.Info($"Functions arrived. {functionsDetails}");

            ValidateFunctionChartingUpdateData(functionChartingUpdates);

            _log.Info($"Functions validated. {functionsDetails}");

            _log.Info($"Functions will be sent for saving. {functionsDetails}");

            //Store function values
            await _functionChartingUpdateRepository.WriteAsync(functionChartingUpdates);

            _log.Info($"Functions saved. {functionsDetails}");

            //Emit function values
            foreach (var function in functionChartingUpdates)
            {
                var functionDetails = JsonConvert.SerializeObject(function);

                _log.Info($"Function {functionDetails} will be sent to RabbitMq");

                await _functionHandler.Handle(Mapper.Map<FunctionChartingUpdate>(function));

                _log.Info($"Function {functionDetails} sent to RabbitMq");
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
