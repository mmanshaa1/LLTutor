namespace App.PL.Helpers
{
    public static class GlobalVariables
    {
        private static IConfiguration configuration;
        public static void Initialize(IConfiguration _configuration)
        {
            configuration = _configuration;
            Reload();
        }

        public static void Reload()
        {
            var GlobalVariables = configuration.GetSection("GlobalVariables");

            ProjectId = GlobalVariables.GetValue<string>("ProjectId");
            BaseUrl = GlobalVariables.GetValue<string>("BaseUrl");
            BaseUrlHome = GlobalVariables.GetValue<string>("BaseUrlHome");
            BaseUrlDashboard = GlobalVariables.GetValue<string>("BaseUrlDashboard");
            AllowAnyOrigin = GlobalVariables.GetValue<bool>("AllowAnyOrigin");
            AllowAnyOrigin = GlobalVariables.GetValue<bool>("IsDevelopment");
        }

        public static string ProjectId { get; set; }
        public static string BaseUrl { get; set; }
        public static string BaseUrlDashboard { get; set; }
        public static string BaseUrlHome { get; set; }
        public static bool AllowAnyOrigin { get; set; }
        public static bool IsDevelopment { get; set; }
    }

    public class EmailCredentials
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
