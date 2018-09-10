using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Common.Log;
using JetBrains.Annotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.AlgoStore.Service.InstanceEventHandler.Filters;
using Lykke.AlgoStore.Service.InstanceEventHandler.Modules;
using Lykke.AlgoStore.Service.InstanceEventHandler.Settings;
using Lykke.Common;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Sdk;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using GlobalErrorHandlerMiddleware = Lykke.AlgoStore.Service.InstanceEventHandler.Infrastructure.GlobalErrorHandlerMiddleware;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Lykke.AlgoStore.Service.InstanceEventHandler
{
    [UsedImplicitly]
    public class Startup
    {
        private const string ApiVersion = "v1";
        private const string ApiName = "Algo Store Instance Event Handler API";

        private string _monitoringServiceUrl;
        private ILog _log;
        private IHealthNotifier _healthNotifier;

        public IContainer ApplicationContainer { get; private set; }
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
            try
            {
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        options.SerializerSettings.ContractResolver =
                            new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    });

                services.AddSwaggerGen(options => { options.DefaultLykkeConfiguration(ApiVersion, ApiName); });

                var settingsManager = Configuration.LoadSettings<AppSettings>(x =>
                {
                    x.SetConnString(y => y.SlackNotifications.AzureQueue.ConnectionString);
                    x.SetQueueName(y => y.SlackNotifications.AzureQueue.QueueName);
                    x.SenderName = $"{AppEnvironment.Name} {AppEnvironment.Version}";
                });

                var appSettings = settingsManager.CurrentValue;

                if (appSettings.MonitoringServiceClient != null)
                    _monitoringServiceUrl = appSettings.MonitoringServiceClient.MonitoringServiceUrl;

                services.AddLykkeLogging(
                    settingsManager.ConnectionString(s => s.AlgoStoreInstanceEventHandlerService.Db.LogsConnectionString),
                    "AlgoInstanceEventHandlerLog",
                    appSettings.SlackNotifications.AzureQueue.ConnectionString,
                    appSettings.SlackNotifications.AzureQueue.QueueName,
                    logBuilder =>
                    {
                        if (Enum.TryParse(appSettings.AlgoStoreInstanceEventHandlerService.LogLevel, true, out LogLevel logLevel))
                            logBuilder.SetMinimumLevel(logLevel);
                        else
                            logBuilder.SetMinimumLevel(LogLevel.Error);
                    });

                services.AddInstanceAuthentication(appSettings.AlgoStoreInstanceEventHandlerService.InstanceAuthSettings);

                services.ConfigureSwaggerGen(options => options.OperationFilter<ApiKeyHeaderOperationFilter>());

                var builder = new ContainerBuilder();

                builder.RegisterModule(new ServiceModule(settingsManager));

                builder.Populate(services);

                ApplicationContainer = builder.Build();

                var logFactory = ApplicationContainer.Resolve<ILogFactory>();
                _log = logFactory.CreateLog(this);
                _healthNotifier = ApplicationContainer.Resolve<IHealthNotifier>();

                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                if (_log == null)
                    Console.WriteLine(ex);
                else
                    _log.Critical(ex);
                throw;
            }
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            try
            {
                app.UseAuthentication();

                app.UseLykkeConfiguration(opt =>
                {
                    opt.WithMiddleware = appBuilder =>
                    {
                        appBuilder.UseMiddleware<GlobalErrorHandlerMiddleware>();
                    };
                });

                appLifetime.ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopped.Register(CleanUp);
            }
            catch(Exception ex)
            {
                _log?.Critical(ex);
                throw;
            }
        }

        private async Task StartApplication()
        {
            try
            {
                _healthNotifier.Notify("Started", Program.EnvInfo);
#if !DEBUG
                await Configuration.RegisterInMonitoringServiceAsync(_monitoringServiceUrl, _healthNotifier);
#endif
            }
            catch (Exception ex)
            {
                _log.Critical(ex);
                throw;
            }
        }

        private void CleanUp()
        {
            try
            {
                // NOTE: Job can't receive and process IsAlive requests here, so you can destroy all resources
                _healthNotifier?.Notify("Terminating", Program.EnvInfo);

                ApplicationContainer.Dispose();
            }
            catch (Exception ex)
            {
                _log?.Critical(ex);
                throw;
            }
        }
    }
}
