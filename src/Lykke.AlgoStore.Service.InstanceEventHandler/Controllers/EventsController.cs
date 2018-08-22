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
            var instance = User.GetInstanceData();

            await _candleService.WriteAsync(instance, candles);

            return NoContent();
        }

        [HttpPost("handleTrades")]
        [SwaggerOperation("HandleTrades")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleTrades([FromBody] List<TradeChartingUpdate> trades)
        {
            var instance = User.GetInstanceData();

            await _tradeService.WriteAsync(instance, trades);

            return NoContent();
        }

        [HttpPost("handleFunctions")]
        [SwaggerOperation("HandleFunctions")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleFunctions([FromBody] List<FunctionChartingUpdate> functions)
        {
            var instance = User.GetInstanceData();

            await _functionService.WriteAsync(instance, functions);

            return NoContent();
        }

        [HttpPost("handleQuotes")]
        [SwaggerOperation("HandleQuotes")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleQuotes([FromBody] List<QuoteChartingUpdate> quotes)
        {
            var instance = User.GetInstanceData();

            await _quoteService.WriteAsync(instance, quotes);

            return NoContent();
        }
    }
}
