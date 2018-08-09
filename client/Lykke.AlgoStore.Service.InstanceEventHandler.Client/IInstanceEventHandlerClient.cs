using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.AlgoStore.Algo.Charting;
using Refit;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Client
{
    /// <summary>
    /// InstanceEventHandler client interface.
    /// </summary>
    [PublicAPI]
    [Headers("Authorization")]
    public interface IInstanceEventHandlerClient
    {
        [Post("/api/v1/events/handleCandles")]
        Task HandleCandlesAsync(List<CandleChartingUpdate> candles);

        [Post("/api/v1/events/handleTrades")]
        Task HandleTradesAsync(List<TradeChartingUpdate> trades);

        [Post("/api/v1/events/handleFunctions")]
        Task HandleFunctionsAsync(List<FunctionChartingUpdate> functions);

        [Post("/api/v1/events/handleQuotes")]
        Task HandleQuotesAsync(List<QuoteChartingUpdate> quotes);
    }
}
