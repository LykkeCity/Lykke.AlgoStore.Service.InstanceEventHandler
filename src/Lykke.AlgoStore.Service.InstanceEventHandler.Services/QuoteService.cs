using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Common;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;
using Lykke.Common.Log;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IHandler<QuoteChartingUpdate> _quoteHandler;
        private readonly BatchSubmitter<QuoteChartingUpdateData> _batchSubmitter;
        private readonly ILog _log;

        public QuoteService(
            IHandler<QuoteChartingUpdate> quoteHandler, 
            ILogFactory logFactory, 
            BatchSubmitter<QuoteChartingUpdateData> batchSubmitter)
        {
            _quoteHandler = quoteHandler;
            _batchSubmitter = batchSubmitter;
            _log = logFactory.CreateLog(this);
        }

        public async Task WriteAsync(
            AlgoClientInstanceData clientInstanceData,
            IEnumerable<QuoteChartingUpdate> quotes)
        {
            var quotesChartingUpdates = quotes.ToList();

            ValidateQuoteChartingUpdateData(clientInstanceData, quotesChartingUpdates);

            _log.Info($"Quotes validated.");

            //Store quote values
            _batchSubmitter.Enqueue(quotes.Select(AutoMapper.Mapper.Map<QuoteChartingUpdateData>));

            _log.Info($"Quotes saved.");

            foreach (var quote in quotesChartingUpdates)
            {
                var quoteDetails = JsonConvert.SerializeObject(quote);

                await _quoteHandler.Handle(quote);
            }

            _log.Info($"Quotes sent to RabbitMQ.");
        }

        private void ValidateQuoteChartingUpdateData(
            AlgoClientInstanceData instance,
            List<QuoteChartingUpdate> quotesChartingUpdateData)
        {
            if (quotesChartingUpdateData == null)
                throw new ArgumentNullException(nameof(quotesChartingUpdateData));

            if (!quotesChartingUpdateData.Any())
                throw new ValidationException(Phrases.QuotesCannotBeEmpty);

            if (quotesChartingUpdateData.Count > 100)
                throw new ValidationException(Phrases.MaxRecordsPerBatchReached);

            if (quotesChartingUpdateData.Select(x => x.InstanceId).Distinct().Count() > 1)
                throw new ValidationException(Phrases.SameInstanceIdForAllQuotesValues);

            if (quotesChartingUpdateData.Any(x => string.IsNullOrEmpty(x.InstanceId)))
                throw new ValidationException(Phrases.InstanceIdForAllQuoteValues);

            var providedInstanceId = quotesChartingUpdateData.Select(x => x.InstanceId).First();

            if (instance.InstanceId != providedInstanceId)
                throw new ValidationException(Phrases.AuthorizationTokenDoesNotCorrespondToProvidedInstanceIds);

            if (quotesChartingUpdateData.Any(x => x.Timestamp == default(DateTime)))
                throw new ValidationException(Phrases.TimestampForQuotes);

            if (quotesChartingUpdateData.Any(x => x.Price <= 0))
                throw new ValidationException(Phrases.PriceForQuotes);

            if (quotesChartingUpdateData.Any(x => String.IsNullOrWhiteSpace(x.AssetPair) ))
                throw new ValidationException(Phrases.AssetPairMustBeSetForQuotes);
        }
    }
}
