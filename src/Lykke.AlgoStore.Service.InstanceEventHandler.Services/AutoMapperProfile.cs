using AutoMapper;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Domain;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ICandle, Candle>();
            CreateMap<ITrade, Trade>();
            CreateMap<IFunction, Function>();
        }
    }
}
