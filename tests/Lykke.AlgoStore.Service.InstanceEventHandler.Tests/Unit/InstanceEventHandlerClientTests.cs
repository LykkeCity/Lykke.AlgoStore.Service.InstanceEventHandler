using System.Linq;
using AutoFixture;
using AutoMapper;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Client;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Tests.Unit
{
    [TestFixture]
    public class InstanceEventHandlerClientTests
    {
        private readonly Fixture _fixture = new Fixture();
        private IInstanceEventHandlerClient _client;

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

            //REMARK: Must use auth token for existing algo instance
            var authHandler = new AlgoAuthorizationHeaderHttpClientHandler("4fba109d-2c42-4b90-9bba-5f75842d012e");

            _client = HttpClientGenerator.HttpClientGenerator
                .BuildForUrl("http://localhost:5000")
                .WithAdditionalDelegatingHandler(authHandler)
                .Create()
                .Generate<IInstanceEventHandlerClient>();
        }

        [Test]
        [Explicit("This test will try to initiate REST API client on localhost. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void HandleCandlesAsync_Test()
        {
            var data = _fixture.Build<CandleChartingUpdate>()
                .With(x => x.InstanceId, "TEST")
                .CreateMany(50)
                .ToList();

            _client.HandleCandlesAsync(data).Wait();
        }

        [Test]
        [Explicit("This test will try to initiate REST API client on localhost. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void HandleTradesAsync_Test()
        {
            var data = _fixture.Build<TradeChartingUpdate>()
                .With(x => x.InstanceId, "TEST")
                .CreateMany(50)
                .ToList();

            _client.HandleTradesAsync(data).Wait();
        }

        [Test]
        [Explicit("This test will try to initiate REST API client on localhost. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void HandleFunctionsAsync_Test()
        {
            var data = _fixture.Build<FunctionChartingUpdate>()
                .With(x => x.InstanceId, "TEST")
                .CreateMany(50)
                .ToList();

            _client.HandleFunctionsAsync(data).Wait();
        }
    }
}
