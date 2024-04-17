using Language.Data;
using Language.Models;
using Microsoft.AspNetCore.Mvc;

namespace Language.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseData _course;

        public CourseController(CourseData courseData)
        {
            _course = courseData;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                List<Category> categories = _course.GetAll();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetAllCoursesByCategory/{category_id}")]
        public IActionResult GetAllCoursesByCategory(Guid category_id)
        {
            try
            {
                List<Course> courses = _course.GetAllCoursesByCategoryId(category_id);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllCategories")]
        public IActionResult GetAllCategories()
        {
            try
            {
                List<Category> categories = _course.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
