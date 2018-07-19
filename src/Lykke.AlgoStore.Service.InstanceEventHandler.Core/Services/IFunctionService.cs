using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Service.InstanceEventHandler.Core.Domain;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Core.Services
{
    public interface IFunctionService
    {
        Task WriteAsync(IEnumerable<IFunction> functions);
    }
}
