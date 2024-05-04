using Language.Data;
using Language.DTOs.MyClass;
using Language.Models;
using Microsoft.AspNetCore.Mvc;

namespace Language.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyClassController : ControllerBase
    {
        private readonly MyClassData _myclass;

        public MyClassController(MyClassData myClassData)
        {
            _myclass = myClassData;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                List<My_Class> my_Classes = _myclass.GetAll();
                return Ok(my_Classes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllByUserId")] 
        public IActionResult GetAllByUserId(Guid user_id)
        {
            try
            {
                List<My_Class> my_Classes = _myclass.GetAllByUserId(user_id);
                return Ok(my_Classes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllByMyClassId")]

        public IActionResult GetAllByMyClassId(Guid class_id) 
        {
            try
            {
                List<My_Class> my_Classes = _myclass.GetAllByClass_id(class_id);
                return Ok(my_Classes);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("CreateMyClass")]
        public IActionResult CreateMyClass([FromBody] MyClassDTO myClassDto)
        {
            if (myClassDto == null)
            {
                return BadRequest("Data should be inputted.");
            }

            My_Class myClass = new My_Class
            {
                class_id = Guid.NewGuid(),
                user_id = myClassDto.user_id,
                detail_invoice_id = myClassDto.detail_invoice_id,
            };

            try
            {
                bool result = _myclass.CreateMyClass(myClass);
                if (result)
                {
                    return StatusCode(201, myClass.class_id);
                }
                else
                {
                    return StatusCode(500, "Failed to create my class.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{class_id}")]
        public IActionResult UpdateMyClass(Guid class_id, [FromBody] MyClassDTO myClassDto)
        {
            if (myClassDto == null)
            {
                return BadRequest("Data should be inputted.");
            }

            My_Class myClass = new My_Class
            {
                user_id = myClassDto.user_id,
                detail_invoice_id = myClassDto.detail_invoice_id,
            };

            bool result = _myclass.UpdateMyClass(class_id, myClass);

            if (result)
            {
                return Ok("My class updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update my class.");
            }
        }

        [HttpDelete("{class_id}")]
        public IActionResult DeleteMyClass(Guid class_id)
        {
            bool result = _myclass.DeleteMyClass(class_id);

            if (result)
            {
                return Ok("My class deleted successfully.");
            }
            else
            {
                return BadRequest("Failed to delete my class.");
            }
        }
    }
}
