using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;
using Lykke.Common.Log;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class TradeService : ITradeService
    {
        private readonly IHandler<TradeChartingUpdate> _tradeHandler;
        private readonly ILog _log;

        public TradeService(IHandler<TradeChartingUpdate> tradeHandler, ILogFactory logFactory)
        {
            _tradeHandler = tradeHandler;
            _log = logFactory.CreateLog(this);
        }

        public async Task WriteAsync(
            AlgoClientInstanceData clientInstanceData,
            IEnumerable<TradeChartingUpdate> trades)
        {
            var tradeChartingUpdates = trades.ToList();

            var tradesDetails = JsonConvert.SerializeObject(tradeChartingUpdates);

            _log.Info($"Trades arrived. {tradesDetails}");

            ValidateTradeChartingUpdateData(clientInstanceData, tradeChartingUpdates);

            _log.Info($"Trades validated. {tradesDetails}");

            foreach (var trade in tradeChartingUpdates)
            {
                var tradeDetails = JsonConvert.SerializeObject(trade);

                _log.Info($"Trade {tradeDetails} will be sent to RabbitMq");

                await _tradeHandler.Handle(trade);

                _log.Info($"Trade {tradeDetails} sent to RabbitMq");
            }
        }

        private void ValidateTradeChartingUpdateData(
            AlgoClientInstanceData instance,
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

            var providedInstanceId = tradeChartingUpdateData.Select(x => x.InstanceId).First();

            if (instance.InstanceId != providedInstanceId)
                throw new ValidationException(Phrases.AuthorizationTokenDoesNotCorrespondToProvidedInstanceIds);

            if (tradeChartingUpdateData.Any(x => string.IsNullOrEmpty(x.Id)))
                throw new ValidationException(Phrases.IdForAllTradeValues);

            if (tradeChartingUpdateData.Any(x => string.IsNullOrEmpty(x.AssetPairId)))
                throw new ValidationException(Phrases.AssetPairIdForAllTradeValues);

            if (tradeChartingUpdateData.Any(x => string.IsNullOrEmpty(x.AssetId)))
                throw new ValidationException(Phrases.AssetIdForAllTradeValues);

            if (tradeChartingUpdateData.Any(x => !x.DateOfTrade.HasValue || x.DateOfTrade == default(DateTime)))
                throw new ValidationException(Phrases.DateOfTradeForAllTradeValues);

            if (tradeChartingUpdateData.Any(x => !x.IsBuy.HasValue))
                throw new ValidationException(Phrases.IsBuyForAllTradeValues);

            if (tradeChartingUpdateData.Any(x => !x.Price.HasValue || (x.Price.HasValue && x.Price.Value <= 0)))
                throw new ValidationException(Phrases.PriceForAllTradeValues);

            if (tradeChartingUpdateData.Any(x => !x.Amount.HasValue || (x.Amount.HasValue && x.Amount.Value <= 0)))
                throw new ValidationException(Phrases.AmountForAllTradeValues);
        }
    }
}
