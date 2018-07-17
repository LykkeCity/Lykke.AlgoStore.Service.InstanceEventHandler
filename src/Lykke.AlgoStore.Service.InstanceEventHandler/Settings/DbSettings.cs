using Lykke.SettingsReader.Attributes;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnectionString { get; set; }

        [AzureTableCheck]
        public string DataStorageConnectionString { get; set; }
    }
}
