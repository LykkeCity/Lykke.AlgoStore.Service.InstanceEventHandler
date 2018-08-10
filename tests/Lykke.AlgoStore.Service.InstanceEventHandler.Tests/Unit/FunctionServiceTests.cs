﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Strings;
using Lykke.Common.Log;
using Moq;
using NUnit.Framework;
using IFunctionChartingUpdateRepository =
    Lykke.AlgoStore.Service.InstanceEventHandler.Core.Repositories.IFunctionChartingUpdateRepository;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Tests.Unit
{
    [TestFixture]
    public class FunctionServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private IFunctionService _service;

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
                _service.WriteAsync(It.IsAny<string>(), new List<FunctionChartingUpdate>()));
        }

        [Test]
        public void WriteAsync_ForRequest_WithMoreThen100Records_WillThrowException_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().CreateMany(101);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithDifferentInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));
        }

        [Test]
        public void WriteAsync_ForRequest_WithEmptyInstanceIds_WillThrowException_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().With(x => x.InstanceId, "").CreateMany(50);
            Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithMultipleRecords_WillSucceed_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(50);
            _service.WriteAsync(It.IsAny<string>(), request).Wait();
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithSingleRecord_WillSucceed_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(1);
            _service.WriteAsync(It.IsAny<string>(), request).Wait();
        }

        [Test]
        public void WriteAsync_ForValidRequest_WithSingleRecord_ThatHasMultipleInnerFunctionValues_WillSucceed_Test()
        {
            var request = _fixture.Build<FunctionChartingUpdate>().With(x => x.InstanceId, "TEST")
                .With(x => x.InnerFunctions,
                    _fixture.Build<FunctionChartingUpdate>().With(x => x.InstanceId, "TEST").CreateMany(3).ToList())
                .CreateMany(1);
            _service.WriteAsync(It.IsAny<string>(), request).Wait();
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

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));

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

            var ex = Assert.ThrowsAsync<ValidationException>(() => _service.WriteAsync(It.IsAny<string>(), request));

            Assert.That(ex.Message, Is.EqualTo(Phrases.CalculatedOnForAllFunctionValues));
        }

        private IFunctionService MockService()
        {
            var handlerMock = new Mock<IHandler<FunctionChartingUpdate>>();
            handlerMock.Setup(x => x.Handle(It.IsAny<FunctionChartingUpdate>())).Returns(Task.CompletedTask);

            var repoMock = new Mock<IFunctionChartingUpdateRepository>();
            repoMock.Setup(x => x.WriteAsync(It.IsAny<IEnumerable<FunctionChartingUpdate>>()))
                .Returns(Task.CompletedTask);

            var logMock = new Mock<ILog>();
            var logFactoryMock = new Mock<ILogFactory>();
            logFactoryMock.Setup(x => x.CreateLog(It.IsAny<object>()))
                .Returns(logMock.Object);

            var algoClientInstanceRepositoryMock = new Mock<IAlgoClientInstanceRepository>();

            algoClientInstanceRepositoryMock.Setup(x => x.GetAlgoInstanceDataByAuthTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Build<AlgoClientInstanceData>().With(x => x.InstanceId, "TEST")
                    .Create()));

            return new FunctionService(handlerMock.Object, repoMock.Object, logFactoryMock.Object,
                algoClientInstanceRepositoryMock.Object);
        }
    }
}
