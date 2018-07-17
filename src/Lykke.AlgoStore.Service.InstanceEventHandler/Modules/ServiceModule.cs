﻿using Autofac;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.InstanceEventHandler.Settings;
using Lykke.Common.Log;
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
        }
    }
}
