using System.Diagnostics;

namespace App.PL.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{details?}")]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error(string? details)
        {
            var exceptionDetails = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            if (exceptionDetails is not null)
            {
                ViewBag.ErrorPath = exceptionDetails.Path;
                ViewBag.ErrorMessage = exceptionDetails.Error.Message;
                ViewBag.ErrorSource = exceptionDetails.Error.Source;
                ViewBag.ErrorStackTrace = exceptionDetails.Error.StackTrace;
            }

            ViewBag.ErrorCode = HttpContext.Response.StatusCode;

            if (details == "404")
                return View("404");


            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
