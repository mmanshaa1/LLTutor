namespace App.APIs.Middlewares
{
    public class RequestDetailsMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestDetailsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Log request details
            LogRequestDetails(context);

            // Call the next middleware in the pipeline
            await _next(context);
        }

        private void LogRequestDetails(HttpContext context)
        {
            var request = context.Request;

            // Get client IP address
            var ipAddress = context.Connection.RemoteIpAddress.ToString();

            // Get device information
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var device = GetDevice(userAgent);

            // Get browser information
            var browser = GetBrowser(userAgent);

            // Get operating system information
            var os = GetOperatingSystem(userAgent);

            // Log or store the information as needed
            var logMessage = $"IP: {ipAddress}, Device: {device}, Browser: {browser}, OS: {os}, RequestPath: {request.Path}";
            Console.WriteLine(logMessage);
        }

        private string GetDevice(string userAgent)
        {
            // Simplified logic, you may want to use a library for more accurate device detection
            return userAgent.Contains("Mobile") ? "Phone" : "PC";
        }

        private string GetBrowser(string userAgent)
        {
            // Simplified logic, you may want to use a library for more accurate browser detection
            if (userAgent.Contains("Edge")) return "Edge";
            if (userAgent.Contains("Chrome")) return "Chrome";
            if (userAgent.Contains("Safari")) return "Safari";
            if (userAgent.Contains("Firefox")) return "Firefox";
            return "Unknown";
        }

        private string GetOperatingSystem(string userAgent)
        {
            // Simplified logic, you may want to use a library for more accurate OS detection
            if (userAgent.Contains("Windows")) return "Windows";
            if (userAgent.Contains("Mac")) return "Mac OS";
            if (userAgent.Contains("Android")) return "Android";
            if (userAgent.Contains("iPhone")) return "iOS";
            return "Unknown";
        }
    }
}
