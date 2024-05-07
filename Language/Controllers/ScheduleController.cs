using Language.Data;
using Language.DTOs.CourseShcedule;
using Language.DTOs.User;
using Language.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Language.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleData _schedule;

        public ScheduleController(ScheduleData scheduleData)
        {
            _schedule = scheduleData;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll() 
        {
            try
            {
                List<Course_Schedule> schedules = _schedule.GetAll();
                return Ok(schedules);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetByCourseId")]
        public IActionResult GetByCourseId(Guid course_id)
        {
            try
            {
                List<Course_Schedule> schedules = _schedule.GetScheduleByCourseId(course_id);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{schedule_id}")]
        public ActionResult<List<Course_Schedule>> GetByScheduleId(Guid schedule_id)
        {
            try
            {
                List<Course_Schedule> schedules = _schedule.GetAllByScheduleId(schedule_id);
                if (schedules.Count == 0)
                {
                    return NotFound(); // Jika tidak ada jadwal dengan ID yang diberikan
                }
                return Ok(schedules); // Mengirimkan data jadwal dalam response
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}"); // Tangani error internal server
            }
        }

        [HttpPost("CreateSchedule")]
        [Authorize(Roles = "admin")]
        public IActionResult CreateSchedule([FromBody] CourseShceduleDTO scheduleDto)
        {
            if (scheduleDto == null)
            {
                return BadRequest("Invalid schedule data.");
            }

            try
            {
                Course_Schedule course_Schedule = new Course_Schedule
                {
                    schedule_id = Guid.NewGuid(),
                    course_id = scheduleDto.course_id,
                    course_date = scheduleDto.course_date
                };
                bool result = _schedule.CreateSchedule(course_Schedule);

                if (result)
                {
                    return StatusCode(201, "Course created successfully.");
                }
                else
                {
                    return BadRequest("Failed to create course.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPut]
        public IActionResult Put(Guid schedule_id, [FromBody] CourseShceduleDTO scheduleDto)
        {

            if (scheduleDto == null)
                return BadRequest("Data Should be Inputed");

            Course_Schedule course_Schedule = new Course_Schedule
            {
                schedule_id = Guid.NewGuid(),
                course_id = scheduleDto.course_id,
                course_date = scheduleDto.course_date

            };

            bool result = _schedule.Update(schedule_id, course_Schedule);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, "error occur");
            }

        }

        [HttpDelete]
        public IActionResult Delete(Guid schedule_id)
        {
            bool result = _schedule.Delete(schedule_id);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, "error occur");
            }
        }
    }
}
