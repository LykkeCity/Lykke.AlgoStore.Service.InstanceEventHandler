using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly object _sync = new object();
        private long _lastDifference = -1;
        private int _duplicateCounter = 99999;

        public static string GeneratePartitionKey(string key) => key;

        public static string GenerateRowKey(long difference, int duplicateCounter) =>
            string.Format("{0:D19}{1:D5}_{2}", difference, duplicateCounter, Guid.NewGuid());

        public FunctionChartingUpdateRepository(INoSQLTableStorage<FunctionChartingUpdateEntity> table)
        {
            _table = table;
        }

        public async Task SaveDifferentPartionsAsync(IEnumerable<FunctionChartingUpdate> data)
        {
            var entities = new List<FunctionChartingUpdateEntity>();
            foreach (var chartingUpdate in data)
            {
                var entity = AutoMapper.Mapper.Map<FunctionChartingUpdateEntity>(chartingUpdate);

                entity.PartitionKey = GeneratePartitionKey(chartingUpdate.InstanceId);
                entity.RowKey = GenerateRowKey();
                entities.Add(entity);
            }

            var groups = entities.GroupBy(p => p.PartitionKey).Select(grp => grp.ToList());

            foreach (var group in groups)
            {
                await _table.InsertOrReplaceAsync(group);
            }
        }

        public async Task WriteAsync(IEnumerable<FunctionChartingUpdate> data)
        {
            var batch = new TableBatchOperation();

            foreach (var chartingUpdate in data)
            {
                var entity = Mapper.Map<FunctionChartingUpdateEntity>(chartingUpdate);

                entity.PartitionKey = GeneratePartitionKey(chartingUpdate.InstanceId);
                entity.RowKey = GenerateRowKey();

                batch.Insert(entity);
            }

            await _table.DoBatchAsync(batch);
        }

        private string GenerateRowKey()
        {
            lock (_sync)
            {
                var difference = DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks;
                if (difference != _lastDifference)
                {
                    _lastDifference = difference;
                    _duplicateCounter = 99999;
                }
                else
                    _duplicateCounter -= 1;

                return GenerateRowKey(difference, _duplicateCounter);
            }
        }
    }
}
