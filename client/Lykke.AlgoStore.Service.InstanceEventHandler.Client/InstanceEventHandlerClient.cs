using Lykke.HttpClientGenerator;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Client
{
    public class InstanceEventHandlerClient : IInstanceEventHandlerClient
    {
        //public IControllerApi Controller { get; }
        
        public InstanceEventHandlerClient(IHttpClientGenerator httpClientGenerator)
        {
            //Controller = httpClientGenerator.Generate<IControllerApi>();
        }
        
    }
}
