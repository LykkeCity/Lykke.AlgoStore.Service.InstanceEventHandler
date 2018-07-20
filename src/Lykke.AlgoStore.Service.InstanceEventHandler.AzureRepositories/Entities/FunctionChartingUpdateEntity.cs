using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories.Entities
{
    public class FunctionChartingUpdateEntity : TableEntity
    {
        public string FunctionName { get; set; }
        public double Value { get; set; }
        public string InstanceId { get; set; }
        public string InnerFunctions { get; set; }
    }
}
