using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Models.Requests
{
    public class Candle : ICandle
    {
        public string InstanceId { get; set; }
    }
}
