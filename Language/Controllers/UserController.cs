using Language.Data;
using Language.DTOs.User;
using Language.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Language.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserData _userData;
        private readonly IConfiguration _configuration;

        public UserController(UserData userData, IConfiguration configuration)
        {
            _userData = userData;
            _configuration = configuration;
        }

        [HttpGet("GetAll")]
        [Authorize]
        public IActionResult GetAll()
        {
            try
            {
                List<User> users = _userData.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetById")]
        public IActionResult Get(Guid id)

        {
            User? user = _userData.GetById(id);

            if (user == null)
            {
                return NotFound("Data Not Found");
            }

            return Ok(user);
        }

        [HttpPost("CreateUser")]

        public IActionResult CreateUser([FromBody] UserDTO userDto)
        {
            try
            {
                User user = new User
                {
                    user_id = Guid.NewGuid(),
                    email = userDto.email,
                    passwords = BCrypt.Net.BCrypt.HashPassword(userDto.passwords),
                };

                UserRole userRole = new UserRole
                {
                    user_id = user.user_id,
                    role = userDto.role
                };

                bool result = _userData.CreateUserAccount(user, userRole);

                if (result)
                {
                    return StatusCode(201, userDto);
                }
                else
                {
                    return StatusCode(500, "Data not inserted");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("login")]

        public IActionResult Login([FromBody] LoginRequestDTO credential)
        {
            if (credential is null) return BadRequest("Invalid client request");

            if (string.IsNullOrEmpty(credential.email) || string.IsNullOrEmpty(credential.passwords)) return BadRequest("Invalid client request");

            User? user = _userData.CheckUserAuth(credential.email);

            if (user == null) return Unauthorized("You do not authorized");

            UserRole? userRole = _userData.GetUserRole(user.user_id);

            bool isVerified = BCrypt.Net.BCrypt.Verify(credential.passwords, user?.passwords);
            //bool isVerified = user?.Password == credential.Password;

            if (user != null && !isVerified)
            {
                return BadRequest("Inccorrect Password! Please check your password");
            }
            else
            {
                var key = _configuration.GetSection("JwtConfig:Key").Value;
                var JwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.email),
                    new Claim(ClaimTypes.Role, userRole.role)
                };

                var signingCredential = new SigningCredentials(
                    JwtKey, SecurityAlgorithms.HmacSha256Signature
                );

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = signingCredential
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var securityToken = tokenHandler.CreateToken(tokenDescriptor);

                string token = tokenHandler.WriteToken(securityToken);

                return Ok(new LoginResponseDTO { Token = token });

            }
        }

        [HttpPut]
        public IActionResult Put(Guid id, [FromBody] UserDTO userDto) // Ubah parameter menjadi int
        {

            if (userDto == null)
                return BadRequest("Data Should be Inputed");

            User user = new User
            {
                user_id = Guid.NewGuid(),
                email = userDto.email,
                passwords = userDto.passwords,
                
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
        public IActionResult Delete(Guid id)
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
