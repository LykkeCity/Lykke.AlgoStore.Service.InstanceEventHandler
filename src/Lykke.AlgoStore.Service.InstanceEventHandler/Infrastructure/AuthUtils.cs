using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Infrastructure
{
    public static class AuthUtils
    {
        public static string GetInstanceAuthToken(this HttpRequest request)
        {
            if (!request.Headers.TryGetValue("Authorization", out var values))
                return string.Empty;

            return values.First().Split(" ").Last();
        }
    }
}
