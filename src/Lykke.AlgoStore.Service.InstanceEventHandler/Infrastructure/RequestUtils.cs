using System.IO;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Infrastructure
{
    //REMARK: This is same class as in Lykke.Common.ApiLibrary
    //Lykke plans to update their common api library and make this public
    //and also, to allow easy override of global error handler middleware
    public static class RequestUtils
    {
        public static async Task<string> GetRequestPartialBodyAsync(HttpContext context)
        {
            if (context?.Request?.Body == null)
            {
                return null;
            }

            // request body might be already read at the moment 
            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            // 64 Kb - is max size for the azure queue message
            const int maxBytesToRead = 1024 * 64;
            var bodyBytes = new byte[maxBytesToRead];
            var bodyBytesCount = await context.Request.Body.ReadAsync(bodyBytes, 0, maxBytesToRead);

            return Encoding.UTF8.GetString(bodyBytes, 0, bodyBytesCount);
        }

        [CanBeNull]
        public static string GetUrlWithoutQuery([CanBeNull] string url)
        {
            if (url == null)
            {
                return null;
            }

            var index = url.IndexOf('?');
            var urlWithoutQuery = index == -1 ? url : url.Substring(0, index);

            return urlWithoutQuery;
        }
    }
}
