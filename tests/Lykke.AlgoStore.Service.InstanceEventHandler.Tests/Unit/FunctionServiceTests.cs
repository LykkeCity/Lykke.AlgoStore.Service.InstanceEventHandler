using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Common;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
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
    public class FunctionServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private IFunctionService _service;
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
                cfg.AddProfile<AutoMapperProfile>();
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
                _service.WriteAsync(_clientInstanceData, new List<FunctionChartingUpdate>()));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMoreThen100Records_WillThrowException_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().CreateMany(101);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithDifferentInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithEmptyInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().With(x => x.InstanceId, "").CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithMultipleRecords_WillSucceed_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(50);
            _service.WriteAsync(_clientInstanceData, request).Wait();
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithSingleRecord_WillSucceed_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(1);
            _service.WriteAsync(_clientInstanceData, request).Wait();
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithSingleRecord_ThatHasMultipleInnerFunctionValues_WillSucceed_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().With(x => x.InstanceId, "TEST")
                .With(x => x.InnerFunctions,
                    _fixture.Build<FunctionChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(3).ToList())
                .CreateMany(1);
            _service.WriteAsync(_clientInstanceData, request).Wait();
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingFunctionNameValue_WillThrowException_Test()
        {
            var request = new List<FunctionChartingUpdate>
            {
                new FunctionChartingUpdate
                {
                    InstanceId = "TEST"
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.FunctionNameForAllFunctionValues));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMissingCalculatedOnValue_WillThrowException_Test()
        {
            var request = new List<FunctionChartingUpdate>
            {
                new FunctionChartingUpdate
                {
                    InstanceId = "TEST",
                    FunctionName = "TEST"
                }
            };

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(_clientInstanceData, request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.CalculatedOnForAllFunctionValues));
        }

        private IFunctionService MockService()
        {
            var handlerMock = new Mock<IHandler<FunctionChartingUpdate>>();
            handlerMock.Setup(x => x.Handle(It.IsAny<FunctionChartingUpdate>())).Returns(Task.CompletedTask);

            var logMock = new Mock<ILog>();
            var logFactoryMock = new Mock<ILogFactory>();
            logFactoryMock.Setup(x => x.CreateLog(It.IsAny<object>()))
                .Returns(logMock.Object);

            var batchSubmitterMock = new BatchSubmitter<FunctionChartingUpdate>
                (TimeSpan.FromHours(1), 100, (data) => Task.CompletedTask);

            return new FunctionService(handlerMock.Object, logFactoryMock.Object, batchSubmitterMock);
        }
    }
}
