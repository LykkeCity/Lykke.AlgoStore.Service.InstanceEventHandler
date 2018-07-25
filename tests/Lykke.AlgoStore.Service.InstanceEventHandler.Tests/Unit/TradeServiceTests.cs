using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoFixture;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services;
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
            Assert.ThrowsAsync<ArgumentNullException>(() => _service.WriteAsync(null));
        }

        [Test]
        public void WriteAsync_ForEmptyRequest_WillThrowException_Test()
        {
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(new List<TradeChartingUpdate>()));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMoreThen100Records_WillThrowException_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().CreateMany(101);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithDifferentInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithEmptyInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().With(x => x.InstanceId, "").CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(request));
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithMultipleRecords_WillSucceed_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(50);
            _service.WriteAsync(request).Wait();
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithSingleRecord_WillSucceed_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(1);
            _service.WriteAsync(request).Wait();
        }

        private static ITradeService MockService()
        {
            var handlerMock = new Mock<IHandler<TradeChartingUpdate>>();
            handlerMock.Setup(x => x.Handle(It.IsAny<TradeChartingUpdate>())).Returns(Task.CompletedTask);

            return new TradeService(handlerMock.Object);
        }
    }
}
