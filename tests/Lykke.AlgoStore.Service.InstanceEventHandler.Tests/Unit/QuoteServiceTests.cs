using AutoFixture;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Common;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;
using Lykke.Common.Log;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Tests.Unit
{
    [TestFixture]
    public class QuoteServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private IQuoteService _service;
        private AlgoClientInstanceData _clientInstanceData;

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
                cfg.AddProfile<AzureRepositories.AutoMapperProfile>();
            });

            Mapper.AssertConfigurationIsValid();

            //Make fixture work with circular references :)
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _service = MockService();

            _clientInstanceData = _fixture.Build<AlgoClientInstanceData>().With(x => x.InstanceId, "TEST").Create();
        }

        [Test]
        public void WriteAsync_ForNullRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.WriteAsync(_clientInstanceData, null));
        }

        [Test]
        public void WriteAsync_ForEmptyRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ValidationException>(() =>
                _service.WriteAsync(_clientInstanceData, new List<QuoteChartingUpdate>()));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMoreThan100Records_WillThrowException_Test()
        {
            var request = _fixture.Build<QuoteChartingUpdate>().CreateMany(101);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithDifferentInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<QuoteChartingUpdate>().CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithEmptyInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<QuoteChartingUpdate>().With(x => x.InstanceId, "").CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithWrongInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<QuoteChartingUpdate>().With(x => x.InstanceId, "FAIL").CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithNegativePrice_WillThrowException_Test()
        {
            var request = _fixture.Build<QuoteChartingUpdate>()
                .With(x => x.InstanceId, "TEST")
                .With(x => x.Timestamp, DateTime.UtcNow)
                .With(x => x.Price, -1)
                .CreateMany(50);

            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithNoTimestamp_WillThrowException_Test()
        {
            var request = _fixture.Build<QuoteChartingUpdate>()
                .With(x => x.InstanceId, "TEST")
                .With(x => x.Price, 1)
                .With(x => x.Timestamp, default(DateTime))
                .CreateMany(50);

            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }


        [Test]
        public void WriteAsync_ForRequest_WithNoAssetPair_WillThrowException_Test()
        {
            var request = _fixture.Build<QuoteChartingUpdate>()
                .With(x => x.InstanceId, "TEST")
                .With(x => x.Price, 1)
                .With(x => x.Timestamp, DateTime.UtcNow)
                .With(x => x.AssetPair, "")
                .CreateMany(50);

            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }
        [Test]
        public void WriteAsync_ForValidRequest_WithMultipleRecords_WillSucceed_Test()
        {
            var request = _fixture.Build<QuoteChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(50);
            _service.WriteAsync(_clientInstanceData, request).Wait();
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithSingleRecord_WillSucceed_Test()
        {
            var request = _fixture.Build<QuoteChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(1);
            _service.WriteAsync(_clientInstanceData, request).Wait();
        }

        private IQuoteService MockService()
        {
            var handlerMock = new Mock<IHandler<QuoteChartingUpdate>>();
            handlerMock.Setup(x => x.Handle(It.IsAny<QuoteChartingUpdate>())).Returns(Task.CompletedTask);

            var logMock = new Mock<ILog>();
            var logFactoryMock = new Mock<ILogFactory>();
            logFactoryMock.Setup(x => x.CreateLog(It.IsAny<object>()))
                .Returns(logMock.Object);

            var batchSubmitterMock = new BatchSubmitter<QuoteChartingUpdateData>
                (TimeSpan.FromHours(1), 100, (data) => Task.CompletedTask);

            return new QuoteService(handlerMock.Object, logFactoryMock.Object, batchSubmitterMock);
        }
    }
}
