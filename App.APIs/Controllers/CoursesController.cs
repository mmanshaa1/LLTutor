namespace App.APIs.Controllers
{
    public class CoursesController : BaseAPIController
    {
        public CoursesController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        // GET: baseUrl/courses
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await unitOfWork.Repository<Course>().GetAllAsync();

            var coursesDto = courses.Select(course => new CourseDto
            {
                Id = course.Id,
                Name = course.Name
            });

            return Ok(coursesDto);
        }
    }
}
