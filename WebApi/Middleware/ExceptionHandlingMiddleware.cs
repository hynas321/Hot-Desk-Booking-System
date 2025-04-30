namespace WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occured for {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\":\"Internal server error\"}");
            }
        }
    }
}
