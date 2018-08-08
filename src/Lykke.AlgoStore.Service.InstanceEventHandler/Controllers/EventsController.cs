using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;
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

        public EventsController(ICandleService candleService, ITradeService tradeService,
            IFunctionService functionService)
        {
            _candleService = candleService;
            _tradeService = tradeService;
            _functionService = functionService;
        }

        [HttpPost("handleCandles")]
        [SwaggerOperation("HandleCandles")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleCandles([FromBody] List<CandleChartingUpdate> candles)
        {
            var authToken = Request.GetInstanceAuthToken();

            await _candleService.WriteAsync(authToken, candles);

            return NoContent();
        }

        [HttpPost("handleTrades")]
        [SwaggerOperation("HandleTrades")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleTrades([FromBody] List<TradeChartingUpdate> trades)
        {
            var authToken = Request.GetInstanceAuthToken();

            await _tradeService.WriteAsync(authToken, trades);

            return NoContent();
        }

        [HttpPost("handleFunctions")]
        [SwaggerOperation("HandleFunctions")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleFunctions([FromBody] List<FunctionChartingUpdate> functions)
        {
            var authToken = Request.GetInstanceAuthToken();

            await _functionService.WriteAsync(authToken, functions);

            return NoContent();
        }
    }
}
