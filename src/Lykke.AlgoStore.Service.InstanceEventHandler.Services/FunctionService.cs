using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Domain;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services;
using Lykke.AlgoStore.Service.InstanceEventHandler.Services.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Services
{
    public class FunctionService : IFunctionService
    {
        private readonly IHandler<Function> _functionHandler;

        public FunctionService(IHandler<Function> functionHandler)
        {
            _functionHandler = functionHandler;
        }

        public async Task WriteAsync(IEnumerable<IFunction> functions)
        {
            foreach (var function in functions)
            {
                await _functionHandler.Handle(Mapper.Map<Function>(function));
            }
        }
    }
}
