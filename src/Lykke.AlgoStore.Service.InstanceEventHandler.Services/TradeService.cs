using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Utils;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class TradeService : ITradeService
    {
        private readonly IHandler<TradeChartingUpdate> _tradeHandler;

        public TradeService(IHandler<TradeChartingUpdate> tradeHandler)
        {
            _tradeHandler = tradeHandler;
        }

        public async Task WriteAsync(IEnumerable<TradeChartingUpdate> trades)
        {
            var tradeChartingUpdates = trades.ToList();

            ValidateTradeChartingUpdateData(tradeChartingUpdates);

            foreach (var trade in tradeChartingUpdates)
            {
                await _tradeHandler.Handle(trade);
            }
        }

        private static void ValidateTradeChartingUpdateData(List<TradeChartingUpdate> tradeChartingUpdateData)
        {
            if(tradeChartingUpdateData == null)
                throw new ArgumentNullException(nameof(tradeChartingUpdateData));

            if (!tradeChartingUpdateData.Any())
                throw new ValidationException(Phrases.TradeValuesCannotBeEmpty);

            if (tradeChartingUpdateData.Count > 100)
                throw new ValidationException(Phrases.MaxRecordsPerBatchReached);

            if (tradeChartingUpdateData.Select(x => x.InstanceId).Distinct().Count() > 1)
                throw new ValidationException(Phrases.SameInstanceIdForAllTradeValues);

            if (tradeChartingUpdateData.Any(x => string.IsNullOrEmpty(x.InstanceId)))
                throw new ValidationException(Phrases.InstanceIdForAllTradeValues);
        }
    }
}
