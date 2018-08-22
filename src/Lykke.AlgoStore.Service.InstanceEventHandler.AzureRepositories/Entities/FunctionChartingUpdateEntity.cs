using System;
using System.Collections.Generic;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories.Entities
{
    public class FunctionChartingUpdateEntity : AzureTableEntity
    {
        public string FunctionName { get; set; }
        public double Value { get; set; }
        public string InstanceId { get; set; }
        public DateTime CalculatedOn { get; set; }
        
        [JsonValueSerializer]
        public List<FunctionChartingUpdate> InnerFunctions { get; set; }
    }
}
