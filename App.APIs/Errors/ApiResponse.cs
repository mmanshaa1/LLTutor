namespace App.APIs.Errors
{
    public class ApiResponse
    {
        public int code { get; set; }
        public string? message { get; set; }

        public ApiResponse(int statusCode, string? Message = null)
        {
            code = statusCode;
            message = Message ?? GetDefaulrMessageForStatusCode(code);
        }

        private string? GetDefaulrMessageForStatusCode(int code)
        {
            return code switch
            {
                400 => "you have made a bad request!",
                401 => "you are not Authorized!",
                404 => "resource was not found!",
                500 => "internal server error!",
                _ => null
            };
        }
    }
}
