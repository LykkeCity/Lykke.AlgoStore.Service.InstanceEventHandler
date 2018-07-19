using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services.Handlers
{
    public class FunctionHandler : IHandler<FunctionChartingUpdate>
    {
        private readonly IHandler<FunctionChartingUpdate> _rabbitMqHandler;

        public FunctionHandler(IHandler<FunctionChartingUpdate> rabbitMqHandler)
        {
            _rabbitMqHandler = rabbitMqHandler;
        }

        public async Task Handle(FunctionChartingUpdate message)
        {
            await _rabbitMqHandler.Handle(message);
        }
    }
}
