using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services.Domain
{
    public class Candle : ICandle
    {
        public string InstanceId { get; set; }
    }
}
