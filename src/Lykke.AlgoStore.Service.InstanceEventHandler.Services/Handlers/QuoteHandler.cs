using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services.Handlers
{
    public class QuoteHandler : IHandler<QuoteChartingUpdate>
    {
        private readonly IHandler<QuoteChartingUpdate> _rabbitMqHandler;

        public QuoteHandler(IHandler<QuoteChartingUpdate> rabbitMqHandler)
        { 
            _rabbitMqHandler = rabbitMqHandler;
        }

        public async Task Handle(QuoteChartingUpdate message)
        {
            await _rabbitMqHandler.Handle(message);
        }
    }
}
