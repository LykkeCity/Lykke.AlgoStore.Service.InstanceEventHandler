using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.Service.InstanceEventHandler.Infrastructure
{
    public class GlobalErrorHandlerMiddleware
    {
        private readonly ILog _log;
        private readonly RequestDelegate _next;

        public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogFactory logFactory)
        {
            if (logFactory == null)
            {
                throw new ArgumentNullException(nameof(logFactory));
            }

            _log = logFactory.CreateLog(this);
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await LogError(context, ex);
                await CreateErrorResponse(context, ex);
            }
        }

        private async Task LogError(HttpContext context, Exception ex)
        {
            var url = context.Request?.GetUri()?.AbsoluteUri;
            var urlWithoutQuery = RequestUtils.GetUrlWithoutQuery(url) ?? "?";
            var body = await RequestUtils.GetRequestPartialBodyAsync(context);

            _log.Error(urlWithoutQuery, ex, null, new { url, body });
        }

        private async Task CreateErrorResponse(HttpContext ctx, Exception ex)
        {
            ctx.Response.ContentType = "application/json";

            string errorMessage;

            switch (ex)
            {
                case ArgumentNullException ane:
                    errorMessage = $"Invalid argument: {ane.Message}";
                    ctx.Response.StatusCode = 400;
                    break;

                case InvalidOperationException ioe:
                    errorMessage = $"Invalid operation: {ioe.Message}";
                    ctx.Response.StatusCode = 400;
                    break;

                case ValidationException ve:
                    errorMessage = $"Validation error: {ve.Message}";
                    ctx.Response.StatusCode = 400;
                    break;

                default:
                    errorMessage = "Technical problem";
                    ctx.Response.StatusCode = 500;
                    break;
            }

            var responseJson = JsonConvert.SerializeObject(errorMessage);

            await ctx.Response.WriteAsync(responseJson);
        }
    }
}
