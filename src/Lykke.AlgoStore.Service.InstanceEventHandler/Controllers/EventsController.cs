using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Controllers
{
    [Authorize]
    [Route("api/v1/events")]
    public class EventsController : Controller
    {
        private readonly ICandleService _candleService;
        private readonly ITradeService _tradeService;
        private readonly IFunctionService _functionService;
        private readonly IQuoteService _quoteService;

        public EventsController(ICandleService candleService, ITradeService tradeService,
            IFunctionService functionService, IQuoteService quoteService)
        {
            _candleService = candleService;
            _tradeService = tradeService;
            _functionService = functionService;
            _quoteService = quoteService;
        }

        [HttpPost("handleCandles")]
        [SwaggerOperation("HandleCandles")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleCandles([FromBody] List<CandleChartingUpdate> candles)
        {
            var authToken = User.GetAuthToken();

            await _candleService.WriteAsync(authToken, candles);

            return NoContent();
        }

        [HttpPost("handleTrades")]
        [SwaggerOperation("HandleTrades")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleTrades([FromBody] List<TradeChartingUpdate> trades)
        {
            var authToken = User.GetAuthToken();

            await _tradeService.WriteAsync(authToken, trades);

            return NoContent();
        }

        [HttpPost("handleFunctions")]
        [SwaggerOperation("HandleFunctions")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleFunctions([FromBody] List<FunctionChartingUpdate> functions)
        {
            var authToken = User.GetAuthToken();

            await _functionService.WriteAsync(authToken, functions);

            return NoContent();
        }

        [HttpPost("handleQuotes")]
        [SwaggerOperation("HandleQuotes")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleQuotes([FromBody] List<QuoteChartingUpdate> quotes)
        {
            var authToken = User.GetAuthToken();

            await _quoteService.WriteAsync(authToken, quotes);

            return NoContent();
        }
    }
}
