using JetBrains.Annotations;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class InstanceEventHandlerSettings
    {
        public DbSettings Db { get; set; }
    }
}
