namespace App.APIs.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BaseAPIController : ControllerBase
    {
        public readonly IUnitOfWork unitOfWork;
        public BaseAPIController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
    }
}
