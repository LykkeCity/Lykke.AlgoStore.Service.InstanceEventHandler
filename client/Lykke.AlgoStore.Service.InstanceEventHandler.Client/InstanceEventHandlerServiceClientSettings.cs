using Lykke.SettingsReader.Attributes;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Client 
{
    /// <summary>
    /// InstanceEventHandler client settings.
    /// </summary>
    public class InstanceEventHandlerServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
