using AutoMapper;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<QuoteChartingUpdate, QuoteChartingUpdateData>();
        }
    }
}
