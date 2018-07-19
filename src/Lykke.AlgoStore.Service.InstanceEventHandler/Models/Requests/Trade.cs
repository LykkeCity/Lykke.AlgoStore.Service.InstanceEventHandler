using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Models.Requests
{
    public class Trade : ITrade
    {
        public string InstanceId { get; set; }
    }
}
