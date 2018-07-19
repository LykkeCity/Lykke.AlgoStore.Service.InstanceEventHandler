using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Models.Requests
{
    public class Function : IFunction
    {
        public string InstanceId { get; set; }
    }
}
