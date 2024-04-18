using Language.Data;
using Language.DTOs.Category;
using Language.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Language.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CourseData _course;

        public CategoryController(CourseData courseData)
        {
            _course = courseData;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllCategories()
        {
            try
            {
                List<Category> categories = _course.GetAllCategories();

                var categoryDetailsList = new List<object>();
                foreach (var category in categories)
                {
                    var categoryDetails = new
                    {
                        category.category_id,
                        category.category_name,
                        category.category_description,
                        category.category_image
                    };
                    categoryDetailsList.Add(categoryDetails);
                }

                return Ok(categoryDetailsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpGet("{category_id}")]
        public IActionResult GetCategory(Guid category_id)
        {
            try
            {
                List<Category> categories = _course.GetByCategoryId(category_id);
                var categoryDetailsList = new List<object>();
                foreach (var category in categories)
                {
                    var categoryDetails = new
                    {
                        category.category_id,
                        category.category_name,
                        category.category_description,
                        category.category_image
                    };
                    categoryDetailsList.Add(categoryDetails);
                }
                return Ok(categoryDetailsList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("CreateCategory")]
        [Authorize(Roles = "admin")]
        public IActionResult CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("Data Should be Inputed");
            }

            Category category = new Category
            {
                category_id = Guid.NewGuid(),
                category_name = categoryDto.category_name,
                category_description = categoryDto.category_description,
                category_image = categoryDto.category_image,

            };

            try
            {
                bool result = _course.CreateCategory(category);
                if (result)
                {
                    return StatusCode(201, category.category_id);
                }
                else
                {
                    return StatusCode(500, "Failed to add detail checkout.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{category_id}")]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateCategory(Guid category_id, [FromBody] CategoryDTO categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("Data Should be Inputed");
            }

            Category category = new Category
            {
                category_id= Guid.NewGuid(),
                category_name= categoryDto.category_name,
                category_description= categoryDto.category_description,
                category_image= categoryDto.category_image,
            };

            bool result = _course.UpdateCategory(category_id, category);

            if (result)
            {
                return Ok("Category updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update category.");
            }
        }

        


    }
}
