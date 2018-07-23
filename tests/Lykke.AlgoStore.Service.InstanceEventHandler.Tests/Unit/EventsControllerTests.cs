using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Controllers;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Tests.Unit
{
    [TestFixture()]
    public class EventsControllerTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<ICandleService> _candleServiceMock;
        private Mock<ITradeService> _tradeServiceMock;
        private Mock<IFunctionService> _functionServiceMock;
        private EventsController _controller;

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

            _candleServiceMock = new Mock<ICandleService>();
            _candleServiceMock.Setup(x => x.WriteAsync(It.IsAny<IEnumerable<CandleChartingUpdate>>()))
                .Returns(Task.CompletedTask);

            _tradeServiceMock = new Mock<ITradeService>();
            _tradeServiceMock.Setup(x => x.WriteAsync(It.IsAny<IEnumerable<TradeChartingUpdate>>()))
                .Returns(Task.CompletedTask);

            _functionServiceMock = new Mock<IFunctionService>();
            _functionServiceMock.Setup(x => x.WriteAsync(It.IsAny<IEnumerable<FunctionChartingUpdate>>()))
                .Returns(Task.CompletedTask);

            _controller = new EventsController(_candleServiceMock.Object, _tradeServiceMock.Object,
                _functionServiceMock.Object);
        }

        [Test]
        public void HandleCandles_WillReturnCorrectResult_Test()
        {
            var result = _controller.HandleCandles(_fixture.Build<List<CandleChartingUpdate>>().Create()).Result;

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void HandleTrades_WillReturnCorrectResult_Test()
        {
            var result = _controller.HandleTrades(_fixture.Build<List<TradeChartingUpdate>>().Create()).Result;

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public void HandleFunctions_WillReturnCorrectResult_Test()
        {
            var result = _controller.HandleFunctions(_fixture.Build<List<FunctionChartingUpdate>>().Create()).Result;

            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
