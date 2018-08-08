using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoFixture;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services;
using Lykke.Common.Log;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Tests.Unit
{
    [TestFixture]
    public class TradeServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private ITradeService _service;

        [SetUp]
        public void SetUp()
        {
            _service = MockService();
        }

        [Test]
        public void WriteAsync_ForNullRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.WriteAsync(It.IsAny<string>(), null));
        }

        [Test]
        public void WriteAsync_ForEmptyRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), new List<TradeChartingUpdate>()));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMoreThen100Records_WillThrowException_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().CreateMany(101);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithDifferentInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithEmptyInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().With(x => x.InstanceId, "").CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithMultipleRecords_WillSucceed_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(50);
            _service.WriteAsync(It.IsAny<string>(), request).Wait();
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithSingleRecord_WillSucceed_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(1);
            _service.WriteAsync(It.IsAny<string>(), request).Wait();
        }

        private ITradeService MockService()
        {
            var handlerMock = new Mock<IHandler<TradeChartingUpdate>>();
            handlerMock.Setup(x => x.Handle(It.IsAny<TradeChartingUpdate>())).Returns(Task.CompletedTask);

            var logMock = new Mock<ILog>();
            var logFactoryMock = new Mock<ILogFactory>();
            logFactoryMock.Setup(x => x.CreateLog(It.IsAny<object>()))
                .Returns(logMock.Object);

            var algoClientInstanceRepositoryMock = new Mock<IAlgoClientInstanceRepository>();

            algoClientInstanceRepositoryMock.Setup(x => x.GetAlgoInstanceDataByAuthTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Build<AlgoClientInstanceData>().With(x => x.InstanceId, "TEST")
                    .Create()));

            return new TradeService(handlerMock.Object, logFactoryMock.Object, algoClientInstanceRepositoryMock.Object);
        }
    }
}
