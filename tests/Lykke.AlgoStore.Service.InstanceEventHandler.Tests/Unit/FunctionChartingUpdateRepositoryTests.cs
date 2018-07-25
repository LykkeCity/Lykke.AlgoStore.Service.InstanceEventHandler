using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using AzureStorage;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories.Entities;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Repositories;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Tests.Unit
{
    [TestFixture]
    public class FunctionChartingUpdateRepositoryTests
    {
        private readonly Fixture _fixture = new Fixture();

        private Mock<INoSQLTableStorage<FunctionChartingUpdateEntity>> _storageMock;
        private FunctionChartingUpdate _functionChartingUpdate;
        private IFunctionChartingUpdateRepository _repository;

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

            //Make fixture work with circular references :)
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _functionChartingUpdate = _fixture.Build<FunctionChartingUpdate>().Create();
            var functionChartingUpdateEntity = Mapper.Map<FunctionChartingUpdateEntity>(_functionChartingUpdate);

            _storageMock = new Mock<INoSQLTableStorage<FunctionChartingUpdateEntity>>();
            _storageMock.Setup(x => x.InsertAsync(functionChartingUpdateEntity)).Returns(Task.CompletedTask);
            _storageMock.Setup(x => x.DoBatchAsync(new TableBatchOperation())).Returns(Task.CompletedTask);

            _repository = new FunctionChartingUpdateRepository(_storageMock.Object);
        }

        [Test]
        public void WriteAsync_MultipleEntities_Test()
        {
            _repository.WriteAsync(
                _fixture.Build<FunctionChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany().ToList()).Wait();
        }
    }
}
