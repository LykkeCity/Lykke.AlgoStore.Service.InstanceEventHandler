using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services.Handlers
{
    public class CandleHandler : IHandler<CandleChartingUpdate>
    {
        private readonly IHandler<CandleChartingUpdate> _rabbitMqHandler;

        public CandleHandler(IHandler<CandleChartingUpdate> rabbitMqHandler)
        {
            _rabbitMqHandler = rabbitMqHandler;
        }

        public async Task Handle(CandleChartingUpdate message)
        {
            await _rabbitMqHandler.Handle(message);
        }
    }
}
