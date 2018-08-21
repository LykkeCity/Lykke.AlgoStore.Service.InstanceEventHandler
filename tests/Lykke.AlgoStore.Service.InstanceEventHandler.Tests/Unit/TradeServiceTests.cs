using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoFixture;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;
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
        private AlgoClientInstanceData _clientInstanceData;

        [SetUp]
        public void SetUp()
        {
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
                _service.WriteAsync(_clientInstanceData, new List<TradeChartingUpdate>()));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMoreThen100Records_WillThrowException_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().CreateMany(101);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithDifferentInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithEmptyInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().With(x => x.InstanceId, "").CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithMultipleRecords_WillSucceed_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(50);
            _service.WriteAsync(_clientInstanceData, request).Wait();
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithSingleRecord_WillSucceed_Test()
        {
            var request = _fixture.Build<TradeChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(1);
            _service.WriteAsync(_clientInstanceData, request).Wait();
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingIdValue_WillThrowException_Test()
        {
            var request = new List<TradeChartingUpdate>
            {
                new TradeChartingUpdate
                {
                    InstanceId = "TEST"
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.IdForAllTradeValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingAssetPairIdValue_WillThrowException_Test()
        {
            var request = new List<TradeChartingUpdate>
            {
                new TradeChartingUpdate
                {
                    InstanceId = "TEST",
                    Id = "TEST"
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.AssetPairIdForAllTradeValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingAssetIdValue_WillThrowException_Test()
        {
            var request = new List<TradeChartingUpdate>
            {
                new TradeChartingUpdate
                {
                    InstanceId = "TEST",
                    Id = "TEST",
                    AssetPairId = "TEST"
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.AssetIdForAllTradeValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingDateOfTradeValue_WillThrowException_Test()
        {
            var request = new List<TradeChartingUpdate>
            {
                new TradeChartingUpdate
                {
                    InstanceId = "TEST",
                    Id = "TEST",
                    AssetPairId = "TEST",
                    AssetId = "TEST",
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.DateOfTradeForAllTradeValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingIsBuyValue_WillThrowException_Test()
        {
            var request = new List<TradeChartingUpdate>
            {
                new TradeChartingUpdate
                {
                    InstanceId = "TEST",
                    Id = "TEST",
                    AssetPairId = "TEST",
                    AssetId = "TEST",
                    DateOfTrade = DateTime.UtcNow
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.IsBuyForAllTradeValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingPriceValue_WillThrowException_Test()
        {
            var request = new List<TradeChartingUpdate>
            {
                new TradeChartingUpdate
                {
                    InstanceId = "TEST",
                    Id = "TEST",
                    AssetPairId = "TEST",
                    AssetId = "TEST",
                    DateOfTrade = DateTime.UtcNow,
                    IsBuy = true
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.PriceForAllTradeValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithZeroPriceValue_WillThrowException_Test()
        {
            var request = new List<TradeChartingUpdate>
            {
                new TradeChartingUpdate
                {
                    InstanceId = "TEST",
                    Id = "TEST",
                    AssetPairId = "TEST",
                    AssetId = "TEST",
                    DateOfTrade = DateTime.UtcNow,
                    IsBuy = true,
                    Price = 0
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.PriceForAllTradeValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithNegativePriceValue_WillThrowException_Test()
        {
            var request = new List<TradeChartingUpdate>
            {
                new TradeChartingUpdate
                {
                    InstanceId = "TEST",
                    Id = "TEST",
                    AssetPairId = "TEST",
                    AssetId = "TEST",
                    DateOfTrade = DateTime.UtcNow,
                    IsBuy = true,
                    Price = -1
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.PriceForAllTradeValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingAmountValue_WillThrowException_Test()
        {
            var request = new List<TradeChartingUpdate>
            {
                new TradeChartingUpdate
                {
                    InstanceId = "TEST",
                    Id = "TEST",
                    AssetPairId = "TEST",
                    AssetId = "TEST",
                    DateOfTrade = DateTime.UtcNow,
                    IsBuy = true,
                    Price = 1
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.AmountForAllTradeValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithZeroAmountValue_WillThrowException_Test()
        {
            var request = new List<TradeChartingUpdate>
            {
                new TradeChartingUpdate
                {
                    InstanceId = "TEST",
                    Id = "TEST",
                    AssetPairId = "TEST",
                    AssetId = "TEST",
                    DateOfTrade = DateTime.UtcNow,
                    IsBuy = true,
                    Price = 1,
                    Amount = 0
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.AmountForAllTradeValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithNegativeAmountValue_WillThrowException_Test()
        {
            var request = new List<TradeChartingUpdate>
            {
                new TradeChartingUpdate
                {
                    InstanceId = "TEST",
                    Id = "TEST",
                    AssetPairId = "TEST",
                    AssetId = "TEST",
                    DateOfTrade = DateTime.UtcNow,
                    IsBuy = true,
                    Price = 1,
                    Amount = -1
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.AmountForAllTradeValues));
        }

        private ITradeService MockService()
        {
            var handlerMock = new Mock<IHandler<TradeChartingUpdate>>();
            handlerMock.Setup(x => x.Handle(It.IsAny<TradeChartingUpdate>())).Returns(Task.CompletedTask);

            var logMock = new Mock<ILog>();
            var logFactoryMock = new Mock<ILogFactory>();
            logFactoryMock.Setup(x => x.CreateLog(It.IsAny<object>()))
                .Returns(logMock.Object);

            return new TradeService(handlerMock.Object, logFactoryMock.Object);
        }
    }
}
