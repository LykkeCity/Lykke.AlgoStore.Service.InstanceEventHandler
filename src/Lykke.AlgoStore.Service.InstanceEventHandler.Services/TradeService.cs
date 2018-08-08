using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;
using Lykke.Common.Log;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class TradeService : ITradeService
    {
        private readonly IHandler<TradeChartingUpdate> _tradeHandler;
        private readonly IAlgoClientInstanceRepository _algoClientInstanceRepository;
        private readonly ILog _log;

        public TradeService(IHandler<TradeChartingUpdate> tradeHandler, ILogFactory logFactory,
            IAlgoClientInstanceRepository algoClientInstanceRepository)
        {
            _tradeHandler = tradeHandler;
            _algoClientInstanceRepository = algoClientInstanceRepository;
            _log = logFactory.CreateLog(this);
        }

        public async Task WriteAsync(string authToken, IEnumerable<TradeChartingUpdate> trades)
        {
            var tradeChartingUpdates = trades.ToList();

            var tradesDetails = JsonConvert.SerializeObject(tradeChartingUpdates);

            _log.Info($"Trades arrived. {tradesDetails}");

            await ValidateTradeChartingUpdateData(authToken, tradeChartingUpdates);

            _log.Info($"Trades validated. {tradesDetails}");

            foreach (var trade in tradeChartingUpdates)
            {
                var tradeDetails = JsonConvert.SerializeObject(trade);

                _log.Info($"Trade {tradeDetails} will be sent to RabbitMq");

                await _tradeHandler.Handle(trade);

                _log.Info($"Trade {tradeDetails} sent to RabbitMq");
            }
        }

        private async Task ValidateTradeChartingUpdateData(string authToken,
            List<TradeChartingUpdate> tradeChartingUpdateData)
        {
            if (tradeChartingUpdateData == null)
                throw new ArgumentNullException(nameof(tradeChartingUpdateData));

            if (!tradeChartingUpdateData.Any())
                throw new ValidationException(Phrases.TradeValuesCannotBeEmpty);

            if (tradeChartingUpdateData.Count > 100)
                throw new ValidationException(Phrases.MaxRecordsPerBatchReached);

            if (tradeChartingUpdateData.Select(x => x.InstanceId).Distinct().Count() > 1)
                throw new ValidationException(Phrases.SameInstanceIdForAllTradeValues);

            if (tradeChartingUpdateData.Any(x => string.IsNullOrEmpty(x.InstanceId)))
                throw new ValidationException(Phrases.InstanceIdForAllTradeValues);

            var instance = await _algoClientInstanceRepository.GetAlgoInstanceDataByAuthTokenAsync(authToken);
            var providedInstanceId = tradeChartingUpdateData.Select(x => x.InstanceId).First();

            if (instance.InstanceId != providedInstanceId)
                throw new ValidationException(Phrases.AuthorizationTokenDoesNotCorrespondToProvidedInstanceIds);
        }
    }
}
