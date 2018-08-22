using System;
using System.Collections.Generic;
using AutoMapper;
using AzureStorage.Tables;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories.Entities;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Repositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Tests.Infrastructure;
using Lykke.Common.Log;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Tests.Unit
{
    [TestFixture]
    public class FunctionChartingUpdateRepositoryLocalStorageTests
    {
        private IFunctionChartingUpdateRepository _repository;
        private Mock<ILogFactory> _logFactory;

        [SetUp]
        public void SetUp()
        {
            //REMARK: http://docs.automapper.org/en/stable/Configuration.html#resetting-static-mapping-configuration
            //Reset should not be used in production code. It is intended to support testing scenarios only.
            Mapper.Reset();

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AutoMapperModelProfile>();
                cfg.AddProfile<Services.AutoMapperProfile>();
                cfg.AddProfile<AutoMapperProfile>();
            });

            Mapper.AssertConfigurationIsValid();

            _logFactory = new Mock<ILogFactory>();

            _repository = new FunctionChartingUpdateRepository(AzureTableStorage<FunctionChartingUpdateEntity>.Create(
                SettingsMock.GetDataStorageConnectionString(), FunctionChartingUpdateRepository.TableName, _logFactory.Object));
        }

        [Test]
        [Explicit("This test will try to write data into local storage. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void FunctionChartingUpdateRepository_SimpleUpdateData_WriteAsync_Test()
        {
            var data = new List<FunctionChartingUpdate>
            {
                new FunctionChartingUpdate
                {
                    FunctionName = $"Function - {DateTime.UtcNow}",
                    InstanceId = $"{Guid.NewGuid()}",
                    Value = (new Random()).NextDouble(),
                    CalculatedOn = DateTime.UtcNow,
                    InnerFunctions = new List<FunctionChartingUpdate>()
                }
            };

            _repository.WriteAsync(data).Wait();
        }

        [Test]
        [Explicit("This test will try to write data into local storage. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void FunctionChartingUpdateRepository_ComplexUpdateData_WriteAsync_Test()
        {
            var data = new List<FunctionChartingUpdate>
            {
                new FunctionChartingUpdate
                {
                    FunctionName = $"Function - {DateTime.UtcNow}",
                    InstanceId = $"{Guid.NewGuid()}",
                    Value = (new Random()).NextDouble(),
                    CalculatedOn = DateTime.UtcNow,
                    InnerFunctions = new List<FunctionChartingUpdate>
                    {
                        new FunctionChartingUpdate
                        {
                            FunctionName = $"Function - {DateTime.UtcNow}",
                            InstanceId = $"{Guid.NewGuid()}",
                            Value = (new Random()).NextDouble(),
                            CalculatedOn = DateTime.UtcNow
                        }
                    }
                }
            };

            _repository.WriteAsync(data).Wait();
        }

        [Test]
        [Explicit("This test will try to write data into local storage. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void FunctionChartingUpdateRepository_MoreComplexUpdateData_WriteAsync_Test()
        {
            var data = new List<FunctionChartingUpdate>
            {
                new FunctionChartingUpdate
                {
                    FunctionName = $"Function - {DateTime.UtcNow}",
                    InstanceId = $"{Guid.NewGuid()}",
                    Value = (new Random()).NextDouble(),
                    CalculatedOn = DateTime.UtcNow,
                    InnerFunctions = new List<FunctionChartingUpdate>
                    {
                        new FunctionChartingUpdate
                        {
                            FunctionName = $"Function - {DateTime.UtcNow}",
                            InstanceId = $"{Guid.NewGuid()}",
                            Value = (new Random()).NextDouble(),
                            CalculatedOn = DateTime.UtcNow,
                            InnerFunctions = new List<FunctionChartingUpdate>
                            {
                                new FunctionChartingUpdate
                                {
                                    FunctionName = $"Function - {DateTime.UtcNow}",
                                    InstanceId = $"{Guid.NewGuid()}",
                                    Value = (new Random()).NextDouble(),
                                    CalculatedOn = DateTime.UtcNow
                                },
                                new FunctionChartingUpdate
                                {
                                    FunctionName = $"Function - {DateTime.UtcNow}",
                                    InstanceId = $"{Guid.NewGuid()}",
                                    Value = (new Random()).NextDouble(),
                                    CalculatedOn = DateTime.UtcNow
                                }
                            }
                        }
                    }
                }
            };

            _repository.WriteAsync(data).Wait();
        }
    }
}
