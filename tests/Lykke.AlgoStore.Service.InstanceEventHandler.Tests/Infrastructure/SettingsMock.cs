using Lykke.AlgoStore.Service.InstanceEventHandler.Settings;
using Lykke.SettingsReader;
using Moq;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Tests.Infrastructure
{
    public class SettingsMock
    {
        public static IReloadingManager<string> GetDataStorageConnectionString()
        {
            var config = InitConfig();

            return config.ConnectionString(x => x.AlgoStoreInstanceEventHandlerService.Db.DataStorageConnectionString);
        }

        private static IReloadingManager<AppSettings> InitConfig()
        {
            var reloadingMock = new Mock<IReloadingManager<AppSettings>>();

            reloadingMock.Setup(x => x.CurrentValue)
                .Returns(new AppSettings
                {
                    AlgoStoreInstanceEventHandlerService = new InstanceEventHandlerSettings
                    {
                        Db = new DbSettings
                        {
                            DataStorageConnectionString = "UseDevelopmentStorage=true",
                            LogsConnectionString = "UseDevelopmentStorage=true"
                        }
                    }
                });

            var config = reloadingMock.Object;

            return config;
        }
    }
}
