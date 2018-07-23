using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class CandleService : ICandleService
    {
        private readonly IHandler<CandleChartingUpdate> _candleHandler;

        public CandleService(IHandler<CandleChartingUpdate> candleHandler)
        {
            _candleHandler = candleHandler;
        }

        public async Task WriteAsync(IEnumerable<CandleChartingUpdate> candles)
        {
            var candlesChartingUpdates = candles.ToList();

            ValidateCandlesChartingUpdateData(candlesChartingUpdates);

            foreach (var candle in candlesChartingUpdates)
            {
                await _candleHandler.Handle(candle);
            }
        }

        private static void ValidateCandlesChartingUpdateData(List<CandleChartingUpdate> candlesChartingUpdateData)
        {
            if (candlesChartingUpdateData == null)
                throw new ArgumentNullException(nameof(candlesChartingUpdateData));

            if (!candlesChartingUpdateData.Any())
                throw new ValidationException(Phrases.CandleValuesCannotBeEmpty);

            if (candlesChartingUpdateData.Count > 100)
                throw new ValidationException(Phrases.MaxRecordsPerBatchReached);

            if (candlesChartingUpdateData.Select(x => x.InstanceId).Distinct().Count() > 1)
                throw new ValidationException(Phrases.SameInstanceIdForAllCandleValues);

            if (candlesChartingUpdateData.Any(x => string.IsNullOrEmpty(x.InstanceId)))
                throw new ValidationException(Phrases.InstanceIdForAllCandleValues);
        }
    }
}
