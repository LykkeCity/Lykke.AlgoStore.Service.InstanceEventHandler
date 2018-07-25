using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
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
            await _candleService.WriteAsync(candles);

            return NoContent();
        }

        [HttpPost("handleTrades")]
        [SwaggerOperation("HandleTrades")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleTrades([FromBody] List<TradeChartingUpdate> trades)
        {
            await _tradeService.WriteAsync(trades);

            return NoContent();
        }

        [HttpPost("handleFunctions")]
        [SwaggerOperation("HandleFunctions")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> HandleFunctions([FromBody] List<FunctionChartingUpdate> functions)
        {
            await _functionService.WriteAsync(functions);

            return NoContent();
        }
    }
}
