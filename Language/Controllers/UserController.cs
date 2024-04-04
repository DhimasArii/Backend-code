using Language.Data;
using Language.DTOs;
using Language.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Language.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserData _userData;

        public UserController(UserData userData)
        {
            _userData = userData;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            List<User> users = _userData.GetAll();
            return Ok(users);
        }

        [HttpGet("GetById")]
        public IActionResult Get(int id) // Ubah parameter menjadi int
        {
            User? user = _userData.GetById(id); // Panggil GetById dengan parameter int

            if (user == null)
            {
                return NotFound("Data Not Found");
            }

            return Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserDTO userDto)
        {

            if (userDto == null)
                return BadRequest("Data Should be Inputed");

            User user = new User
            {
                // user_id tidak perlu diisi di sini karena akan diisi oleh database
                email = userDto.email,
                password = userDto.password,
                address = userDto.address,
                phone_number = userDto.phone_number,
            };

            bool result = _userData.Insert(user);

            if (result)
            {
                return StatusCode(201, user.user_id); // user_id sudah berupa int
            }
            else
            {
                return StatusCode(500, "error occur");
            }

        }

        [HttpPut]
        public IActionResult Put(int id, [FromBody] UserDTO userDto) // Ubah parameter menjadi int
        {

            if (userDto == null)
                return BadRequest("Data Should be Inputed");

            User user = new User
            {
                // user_id tidak perlu diisi di sini karena akan diisi oleh parameter int id
                email = userDto.email,
                password = userDto.password,
                address = userDto.address,
                phone_number = userDto.phone_number,
            };

            bool result = _userData.Update(id, user);

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
        public IActionResult Delete(int id) // Ubah parameter menjadi int
        {
            bool result = _userData.Delete(id);

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
