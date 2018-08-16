using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.AlgoStore.Algo.Charting;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services
{
    public interface IQuoteService
    {
        Task WriteAsync(string authToken, IEnumerable<QuoteChartingUpdate> quotes);
    }
}
