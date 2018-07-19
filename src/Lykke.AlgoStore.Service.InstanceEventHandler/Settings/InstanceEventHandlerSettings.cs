using JetBrains.Annotations;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.AlgoStore.Service.InstanceEventHandler.Settings.RealTimeData;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class InstanceEventHandlerSettings
    {
        public DbSettings Db { get; set; }
        public InstanceAuthSettings InstanceAuthSettings { get; set; }
        public RateLimitSettings RateLimitSettings { get; set; }
        public RealTimeDataSettings RealTimeDataStreamingSettings { get; set; }
    }
}
