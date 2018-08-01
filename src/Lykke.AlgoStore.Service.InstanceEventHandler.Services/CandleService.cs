﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;
using Lykke.Common.Log;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class CandleService : ICandleService
    {
        private readonly IHandler<CandleChartingUpdate> _candleHandler;
        private readonly ILog _log;

        public CandleService(IHandler<CandleChartingUpdate> candleHandler, ILogFactory logFactory)
        {
            _candleHandler = candleHandler;
            _log = logFactory.CreateLog(this);
        }

        public async Task WriteAsync(IEnumerable<CandleChartingUpdate> candles)
        {
            var candlesChartingUpdates = candles.ToList();

            var candlesDetails = JsonConvert.SerializeObject(candlesChartingUpdates);

            _log.Info($"Candles arrived. {candlesDetails}");

            ValidateCandlesChartingUpdateData(candlesChartingUpdates);

            _log.Info($"Candles validated. {candlesDetails}");

            foreach (var candle in candlesChartingUpdates)
            {
                var candleDetails = JsonConvert.SerializeObject(candle);

                _log.Info($"Candle {candleDetails} will be sent to RabbitMq");

                await _candleHandler.Handle(candle);

                _log.Info($"Candle {candleDetails} sent to RabbitMq");
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
