using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Service.InstanceEventHandler.AzureRepositories;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using NUnit.Framework;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Tests.Unit
{
    [TestFixture]
    public class InstanceRabbitMqSubscriptionTests
    {
        private readonly Fixture _fixture = new Fixture();
        private RabbitMqSubscriber<CandleChartingUpdate> _candleSubscriber;
        private RabbitMqSubscriber<TradeChartingUpdate> _tradeSubscriber;
        private RabbitMqSubscriber<FunctionChartingUpdate> _functionSubscriber;
        private ILogFactory _logFactory;

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

            _logFactory = LogFactory.Create().AddConsole();

            var candleSubscriberSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = "amqp://lykke.history:lykke.history@rabbit-me.lykke-me.svc.cluster.local:5672",
                ExchangeName = "lykke.algostore.instance-event-handler.candles",
                QueueName = "CandleSubsriberTestQueue"
            };

            var tradeSubscriberSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = "amqp://lykke.history:lykke.history@rabbit-me.lykke-me.svc.cluster.local:5672",
                ExchangeName = "lykke.algostore.instance-event-handler.trades",
                QueueName = "TradeSubsriberTestQueue"
            };

            var functionSubscriberSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = "amqp://lykke.history:lykke.history@rabbit-me.lykke-me.svc.cluster.local:5672",
                ExchangeName = "lykke.algostore.instance-event-handler.functions",
                QueueName = "FunctionSubsriberTestQueue"
            };

            _candleSubscriber = new RabbitMqSubscriber<CandleChartingUpdate>(
                    _logFactory,
                    candleSubscriberSettings,
                    new ResilientErrorHandlingStrategy(_logFactory, candleSubscriberSettings,
                        TimeSpan.FromMilliseconds(5000), 5,
                        new DeadQueueErrorHandlingStrategy(_logFactory, candleSubscriberSettings)))
                .SetMessageDeserializer(new MessagePackMessageDeserializer<CandleChartingUpdate>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .CreateDefaultBinding()
                .Subscribe(OnCandleChartingUpdateReceived);

            _tradeSubscriber = new RabbitMqSubscriber<TradeChartingUpdate>(
                    _logFactory,
                    tradeSubscriberSettings,
                    new ResilientErrorHandlingStrategy(_logFactory, tradeSubscriberSettings,
                        TimeSpan.FromMilliseconds(5000), 5,
                        new DeadQueueErrorHandlingStrategy(_logFactory, tradeSubscriberSettings)))
                .SetMessageDeserializer(new MessagePackMessageDeserializer<TradeChartingUpdate>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .CreateDefaultBinding()
                .Subscribe(OnTradeChartingUpdateReceived);

            _functionSubscriber = new RabbitMqSubscriber<FunctionChartingUpdate>(
                    _logFactory,
                    functionSubscriberSettings,
                    new ResilientErrorHandlingStrategy(_logFactory, functionSubscriberSettings,
                        TimeSpan.FromMilliseconds(5000), 5,
                        new DeadQueueErrorHandlingStrategy(_logFactory, functionSubscriberSettings)))
                .SetMessageDeserializer(new MessagePackMessageDeserializer<FunctionChartingUpdate>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .CreateDefaultBinding()
                .Subscribe(OnFunctionChartingUpdateReceived);
        }

        [Test]
        [Explicit("This test will create RabbitMq subscription for candles exchange. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void Subscribe_ForCandleChartUpdate_Test()
        {
            _candleSubscriber.Start();

            //Lets wait a little bit so that we can check if new queue is created (via RabbitMq UI)
            Task.Delay(60000).Wait();
        }

        [Test]
        [Explicit("This test will create RabbitMq subscription for trades exchange. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void Subscribe_ForTradeChartUpdate_Test()
        {
            _tradeSubscriber.Start();

            //Lets wait a little bit so that we can check if new queue is created (via RabbitMq UI)
            Task.Delay(60000).Wait();
        }

        [Test]
        [Explicit("This test will create RabbitMq subscription for functions exchange. Do not remove explicit attribute ever and use this just for local testing :)")]
        public void Subscribe_ForFunctionChartUpdate_Test()
        {
            _functionSubscriber.Start();

            //Lets wait a little bit so that we can check if new queue is created (via RabbitMq UI)
            Task.Delay(60000).Wait();
        }

        private Task OnFunctionChartingUpdateReceived(FunctionChartingUpdate function)
        {
            return Task.CompletedTask;
        }

        private Task OnTradeChartingUpdateReceived(TradeChartingUpdate trade)
        {
            return Task.CompletedTask;
        }

        private Task OnCandleChartingUpdateReceived(CandleChartingUpdate candle)
        {
            return Task.CompletedTask;
        }
    }
}
