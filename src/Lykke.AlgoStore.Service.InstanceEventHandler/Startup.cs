using System;
using JetBrains.Annotations;
using Lykke.AlgoStore.Service.InstanceEventHandler.Settings;
using Lykke.Sdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Service.InstanceEventHandler
{
    [UsedImplicitly]
    public class Startup
    {
        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {                                   
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "InstanceEventHandler API";
                options.Logs = logs =>
                {
                    logs.AzureTableName = "InstanceEventHandlerLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.AlgoStoreInstanceEventHandlerService.Db.LogsConnectionString;

                    // TODO: You could add extended logging configuration here:
                    /* 
                    logs.Extended = extendedLogs =>
                    {
                        // For example, you could add additional slack channel like this:
                        extendedLogs.AddAdditionalSlackChannel("InstanceEventHandler", channelOptions =>
                        {
                            channelOptions.MinLogLevel = LogLevel.Information;
                        });
                    };
                    */
                };

                // TODO: You could add extended Swagger configuration here:
                /*
                options.Swagger = swagger =>
                {
                    swagger.IgnoreObsoleteActions();
                };
                */
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration();

        }
    }
}
