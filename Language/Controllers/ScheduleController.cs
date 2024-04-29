using Language.Data;
using Language.DTOs.CourseShcedule;
using Language.DTOs.User;
using Language.Models;
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

        [HttpPost]
        public IActionResult CreateSchedule([FromBody] CourseShceduleDTO scheduleDto)
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
                return Ok("Course created successfully.");
            }
            else
            {
                return BadRequest("Failed to create course.");
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
