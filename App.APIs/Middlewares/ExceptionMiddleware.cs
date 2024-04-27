namespace App.APIs.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHostEnvironment env;
        private readonly ILogger<ExceptionMiddleware> logger;

        public ExceptionMiddleware(RequestDelegate next_, ILogger<ExceptionMiddleware> logger_, IHostEnvironment env_)
        {
            next = next_;
            env = env_;
            logger = logger_;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                ApiExceptionResponse response;
                if (env.IsDevelopment())
                {
                    response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString());
                }
                else
                {
                    response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request.", null);
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
