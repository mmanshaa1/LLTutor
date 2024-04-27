namespace App.PL.Helpers
{
    public class IZITechsErrorLogger
    {
        public static async Task LogError(ExceptionDTO ex)
        {
            using var client = new HttpClient();
            await client.PostAsJsonAsync("https://projects.izitechs.com/Exceptions", ex);
        }
    }

    public class ExceptionDTO
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string? ProjectId { get; set; } = GlobalVariables.ProjectId;
        public int Source { get; set; } = 1;
    }
}
