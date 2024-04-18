using Language.Data;
using Language.DTOs.Category;
using Language.DTOs.Course;
using Language.DTOs.DetailCheckout;
using Language.Models;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("GetAllCourse")]
        public IActionResult GetAllCourses()
        {
            try
            {
                List<Course> courses = _course.GetAllCourses();
                if (courses == null || courses.Count == 0)
                {
                    return NotFound("No courses found.");
                }

                return Ok(courses);
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

        [HttpGet("{course_id}")]
        public IActionResult GetAllByCourseId(Guid course_id)
        {
            try
            {
                List<Course> courses = _course.GetAllByCourseId(course_id);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateCourse([FromBody] Course course)
        {
            bool result = _course.CreateCourse(course);

            if (result)
            {
                return Ok("Course created successfully.");
            }
            else
            {
                return BadRequest("Failed to create course.");
            }
        }

        [HttpPut("{course_id}")]
        public IActionResult UpdateCourse(Guid course_id, [FromBody] CourseDTO courseDto)
        {
            if (courseDto == null)
            {
                return BadRequest("Data Should be Inputed");
            }

            Course courses = new Course
            {
                course_id = Guid.NewGuid(),
                category_id = courseDto.category_id,
                course_name = courseDto.course_name,
                course_description = courseDto.course_description,
                course_image = courseDto.course_image,
                price = courseDto.price,
            };

            bool result = _course.UpdateCourse(course_id, courses);

            if (result)
            {
                return Ok("Course updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update course.");
            }
        }

        /*[HttpDelete("{course_id}")]
        public IActionResult DeleteCourse(Guid course_id)
        {
            bool result = _course.DeleteCourse(course_id);

            if (result)
            {
                return Ok("Course deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete course.");
            }
        }*/



    }
}
