﻿using Autofac;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Domain;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Handlers;
using Lykke.AlgoStore.Service.InstanceEventHandler.Settings;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Modules
{    
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;
        
        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them

            var reloadingDbManager = _appSettings.ConnectionString(x => x.AlgoStoreInstanceEventHandlerService.Db.DataStorageConnectionString);

            builder.RegisterGeneric(typeof(RabbitMqHandler<>));

            builder.Register(x =>
                {
                    var log = x.Resolve<ILogFactory>().CreateLog(this);

                    var algoClientInstanceRepository =
                        AzureRepoFactories.CreateAlgoClientInstanceRepository(reloadingDbManager, log);

                    return algoClientInstanceRepository;
                })
                .As<IAlgoClientInstanceRepository>()
                .SingleInstance();

            builder.RegisterInstance(_appSettings.CurrentValue.AlgoStoreInstanceEventHandlerService.RateLimitSettings)
                .AsSelf()
                .SingleInstance();

            var rabbitMqCandlesSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _appSettings.CurrentValue.AlgoStoreInstanceEventHandlerService
                    .RealTimeDataStreamingSettings.RabbitMqSources.Candles.ConnectionString,
                ExchangeName = _appSettings.CurrentValue.AlgoStoreInstanceEventHandlerService
                    .RealTimeDataStreamingSettings.RabbitMqSources.Candles.ExchangeName,
                IsDurable = true
            };

            var rabbitMqTradesSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _appSettings.CurrentValue.AlgoStoreInstanceEventHandlerService
                    .RealTimeDataStreamingSettings.RabbitMqSources.Trades.ConnectionString,
                ExchangeName = _appSettings.CurrentValue.AlgoStoreInstanceEventHandlerService
                    .RealTimeDataStreamingSettings.RabbitMqSources.Trades.ExchangeName,
                IsDurable = true
            };

            var rabbitMqFunctionsSettings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _appSettings.CurrentValue.AlgoStoreInstanceEventHandlerService
                    .RealTimeDataStreamingSettings.RabbitMqSources.Functions.ConnectionString,
                ExchangeName = _appSettings.CurrentValue.AlgoStoreInstanceEventHandlerService
                    .RealTimeDataStreamingSettings.RabbitMqSources.Functions.ExchangeName,
                IsDurable = true
            };

            var logFactory = LogFactory.Create().AddConsole();

            RegisterRabbitMqHandler<Candle>(builder, rabbitMqCandlesSettings, logFactory, "candleHandler");
            RegisterRabbitMqHandler<Trade>(builder, rabbitMqTradesSettings, logFactory, "tradeHandler");
            RegisterRabbitMqHandler<Function>(builder, rabbitMqFunctionsSettings, logFactory, "functionHandler");

            builder.RegisterType<CandleHandler>()
                .WithParameter((info, context) => info.Name == "rabbitMqHandler",
                    (info, context) => context.ResolveNamed<IHandler<Candle>>("candleHandler"))
                .SingleInstance()
                .As<IHandler<Candle>>();

            builder.RegisterType<TradeHandler>()
                .WithParameter((info, context) => info.Name == "rabbitMqHandler",
                    (info, context) => context.ResolveNamed<IHandler<Trade>>("tradeHandler"))
                .SingleInstance()
                .As<IHandler<Trade>>();

            builder.RegisterType<FunctionHandler>()
                .WithParameter((info, context) => info.Name == "rabbitMqHandler",
                    (info, context) => context.ResolveNamed<IHandler<Function>>("functionHandler"))
                .SingleInstance()
                .As<IHandler<Function>>();

            builder.RegisterType<CandleService>().As<ICandleService>().SingleInstance();
            builder.RegisterType<TradeService>().As<ITradeService>().SingleInstance();
            builder.RegisterType<FunctionService>().As<IFunctionService>().SingleInstance();
        }

        private static void RegisterRabbitMqHandler<T>(ContainerBuilder container,
            RabbitMqSubscriptionSettings exchangeConfiguration, ILogFactory logFactory, string regKey = "")
        {
            container.RegisterType<RabbitMqHandler<T>>()
                .WithParameter("rabbitMqSettings", exchangeConfiguration)
                .WithParameter("logFactory", logFactory)
                .Named<IHandler<T>>(regKey)
                .As<IHandler<T>>();
        }
    }
}
