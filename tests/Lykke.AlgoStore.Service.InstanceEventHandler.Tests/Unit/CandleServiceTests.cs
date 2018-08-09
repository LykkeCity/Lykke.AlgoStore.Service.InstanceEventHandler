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
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;
using Lykke.Common.Log;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Tests.Unit
{
    [TestFixture]
    public class CandleServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private ICandleService _service;

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
            Assert.ThrowsAsync<ValidationException>(() =>
                _service.WriteAsync(It.IsAny<string>(), new List<CandleChartingUpdate>()));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMoreThen100Records_WillThrowException_Test()
        {
            var request = _fixture.Build<CandleChartingUpdate>().CreateMany(101);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithDifferentInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<CandleChartingUpdate>().CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithEmptyInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<CandleChartingUpdate>().With(x => x.InstanceId, "").CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithMultipleRecords_WillSucceed_Test()
        {
            var request = _fixture.Build<CandleChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(50);
            _service.WriteAsync(It.IsAny<string>(), request).Wait();
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithSingleRecord_WillSucceed_Test()
        {
            var request = _fixture.Build<CandleChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(1);
            _service.WriteAsync(It.IsAny<string>(), request).Wait();
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingDateTime_WillThrowException_Test()
        {
            var request = new List<CandleChartingUpdate>
            {
                new CandleChartingUpdate
                {
                    InstanceId = "TEST"
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.DateTimeForAllCandleValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingOpenValue_WillThrowException_Test()
        {
            var request = new List<CandleChartingUpdate>
            {
                new CandleChartingUpdate
                {
                    InstanceId = "TEST",
                    DateTime = DateTime.UtcNow
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.OpenForAllCandleValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingCloseValue_WillThrowException_Test()
        {
            var request = new List<CandleChartingUpdate>
            {
                new CandleChartingUpdate
                {
                    InstanceId = "TEST",
                    DateTime = DateTime.UtcNow,
                    Open = 1
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.CloseForAllCandleValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingHighValue_WillThrowException_Test()
        {
            var request = new List<CandleChartingUpdate>
            {
                new CandleChartingUpdate
                {
                    InstanceId = "TEST",
                    DateTime = DateTime.UtcNow,
                    Open = 1,
                    Close = 1
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.HighForAllCandleValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingLowValue_WillThrowException_Test()
        {
            var request = new List<CandleChartingUpdate>
            {
                new CandleChartingUpdate
                {
                    InstanceId = "TEST",
                    DateTime = DateTime.UtcNow,
                    Open = 1,
                    Close = 1,
                    High = 1
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.LowForAllCandleValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingTradingVolumeValue_WillThrowException_Test()
        {
            var request = new List<CandleChartingUpdate>
            {
                new CandleChartingUpdate
                {
                    InstanceId = "TEST",
                    DateTime = DateTime.UtcNow,
                    Open = 1,
                    Close = 1,
                    High = 1,
                    Low = 1
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.TradingVolumeForAllCandleValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingTradingOppositeVolumeValue_WillThrowException_Test()
        {
            var request = new List<CandleChartingUpdate>
            {
                new CandleChartingUpdate
                {
                    InstanceId = "TEST",
                    DateTime = DateTime.UtcNow,
                    Open = 1,
                    Close = 1,
                    High = 1,
                    Low = 1,
                    TradingVolume = 1
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.TradingOppositeVolumeForAllCandleValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingLastTradePriceValue_WillThrowException_Test()
        {
            var request = new List<CandleChartingUpdate>
            {
                new CandleChartingUpdate
                {
                    InstanceId = "TEST",
                    DateTime = DateTime.UtcNow,
                    Open = 1,
                    Close = 1,
                    High = 1,
                    Low = 1,
                    TradingVolume = 1,
                    TradingOppositeVolume = 1
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.LastTradePriceForAllCandleValues));
        }

        private ICandleService MockService()
        {
            var handlerMock = new Mock<IHandler<CandleChartingUpdate>>();
            handlerMock.Setup(x => x.Handle(It.IsAny<CandleChartingUpdate>())).Returns(Task.CompletedTask);

            var logMock = new Mock<ILog>();
            var logFactoryMock = new Mock<ILogFactory>();
            logFactoryMock.Setup(x => x.CreateLog(It.IsAny<object>()))
                .Returns(logMock.Object);

            var algoClientInstanceRepositoryMock = new Mock<IAlgoClientInstanceRepository>();

            algoClientInstanceRepositoryMock.Setup(x => x.GetAlgoInstanceDataByAuthTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Build<AlgoClientInstanceData>().With(x => x.InstanceId, "TEST")
                    .Create()));

            return new CandleService(handlerMock.Object, logFactoryMock.Object,
                algoClientInstanceRepositoryMock.Object);
        }
    }
}
