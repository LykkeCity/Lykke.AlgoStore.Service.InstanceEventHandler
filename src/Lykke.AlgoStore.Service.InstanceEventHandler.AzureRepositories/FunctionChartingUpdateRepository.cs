using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories.Entities;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories
{
    public class FunctionChartingUpdateRepository : IFunctionChartingUpdateRepository
    {
        private readonly INoSQLTableStorage<FunctionChartingUpdateEntity> _table;

        public static readonly string TableName = "AlgoInstanceFunctionsChartingTable";

        public static string GeneratePartitionKey(string key) => key;

        public static string GenerateRowKey(string key) => key;

        public FunctionChartingUpdateRepository(INoSQLTableStorage<FunctionChartingUpdateEntity> table)
        {
            _table = table;
        }

        public async Task WriteAsync(FunctionChartingUpdate data)
        {
            var entity = Mapper.Map<FunctionChartingUpdateEntity>(data);

            entity.PartitionKey = GeneratePartitionKey(data.InstanceId);
            entity.RowKey = GenerateRowKey(data.FunctionName);

            await _table.InsertAsync(entity);
        }

        public async Task WriteAsync(IEnumerable<FunctionChartingUpdate> data)
        {
            var batch = new TableBatchOperation();

            foreach (var chartingUpdate in data)
            {
                var entity = Mapper.Map<FunctionChartingUpdateEntity>(chartingUpdate);

                entity.PartitionKey = GeneratePartitionKey(chartingUpdate.InstanceId);
                entity.RowKey = GenerateRowKey(chartingUpdate.FunctionName);

                batch.Insert(entity);
            }

            await _table.DoBatchAsync(batch);
        }
    }
}
