using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public InstanceEventHandlerSettings AlgoStoreInstanceEventHandlerService { get; set; }        
    }
}
