namespace App.APIs.Controllers
{
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        public ActionResult Error(int code)
        {
            if (code == 401) return Unauthorized(new ApiResponse(code));
            else if (code == 404) return NotFound(new ApiResponse(code, "Not-Found End-Point"));
            else if (code == 400) return BadRequest(new ApiResponse(code));
            else if (code == 403) return Forbid("Forbidden, You don’t have permission to access!");
            else if (code == 405) return BadRequest(new ApiResponse(code));
            else if (code == 500) return BadRequest(new ApiResponse(code));
            return BadRequest(new ApiResponse(code));
        }
    }
}
