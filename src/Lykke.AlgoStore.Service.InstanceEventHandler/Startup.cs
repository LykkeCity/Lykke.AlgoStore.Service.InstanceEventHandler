using System;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.AlgoStore.Service.InstanceEventHandler.Filters;
using Lykke.AlgoStore.Service.InstanceEventHandler.Settings;
using Lykke.Sdk;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Service.InstanceEventHandler
{
    [UsedImplicitly]
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            InitMapper();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static void InitMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<AutoMapperModelProfile>();
                cfg.AddProfile<Services.AutoMapperProfile>();
                cfg.AddProfile<AzureRepositories.AutoMapperProfile>();
            });

            Mapper.AssertConfigurationIsValid();
        }

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.LoadSettings<AppSettings>();

            services.AddInstanceAuthentication(appSettings.CurrentValue.AlgoStoreInstanceEventHandlerService.InstanceAuthSettings);

            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = new LykkeSwaggerOptions
                {
                    ApiTitle = "Algo Store Instance Event Handler API"
                };

                options.Logs = logs =>
                {
                    logs.AzureTableName = "AlgoInstanceEventHandlerLog";
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
                options.Swagger = swagger =>
                {
                    swagger.OperationFilter<ApiKeyHeaderOperationFilter>();
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();

            app.UseLykkeConfiguration();
        }
    }
}
