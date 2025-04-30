using Microsoft.AspNetCore.Mvc.Controllers;
using System.Text;

namespace WebApi.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            Endpoint endpoint = context.GetEndpoint();
            ControllerActionDescriptor descriptor =
                endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (descriptor == null)
            {
                await _next(context);
                return;
            }

            string controllerName = descriptor.ControllerName;
            string actionName = descriptor.ActionName;

            context.Request.EnableBuffering();
            Stream requestBodyStream = context.Request.Body;
            string requestBody = await ReadStreamAsync(requestBodyStream);
            requestBodyStream.Position = 0;

            _logger.LogInformation(
                "REQUEST {Method} {Path} (Controller: {Controller}/{Action})\n" +
                "Headers: {@Headers}\nBody: {Body}",
                context.Request.Method,
                context.Request.Path + context.Request.QueryString,
                controllerName,
                actionName,
                context.Request.Headers,
                requestBody);

            Stream originalResponseBody = context.Response.Body;
            MemoryStream responseBuffer = new MemoryStream();
            context.Response.Body = responseBuffer;

            await _next(context);

            responseBuffer.Seek(0, SeekOrigin.Begin);
            string responseBody = await ReadStreamAsync(responseBuffer);
            responseBuffer.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation(
                "RESPONSE {StatusCode} (Controller: {Controller}/{Action})\n" +
                "Headers: {@Headers}\nBody: {Body}",
                context.Response.StatusCode,
                controllerName,
                actionName,
                context.Response.Headers,
                responseBody);

            await responseBuffer.CopyToAsync(originalResponseBody);
        }

        private static async Task<string> ReadStreamAsync(Stream stream)
        {
            StreamReader reader = new StreamReader(
                stream,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);
            string content = await reader.ReadToEndAsync();
            return content;
        }
    }
}
