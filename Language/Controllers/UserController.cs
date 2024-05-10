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
using Language.DTOs.Email;
using Microsoft.AspNetCore.WebUtilities;
using Language.DTOs.ForgetPassword;
using Microsoft.AspNetCore.Identity;

namespace Language.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserData _userData;
        private readonly IConfiguration _configuration;
        private readonly EmailService _mail;

        public UserController(UserData userData, IConfiguration configuration, EmailService mail)
        {
            _userData = userData;
            _configuration = configuration;
            _mail = mail;
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
        public async Task<IActionResult> CreateUserAsync([FromBody] UserDTO userDto)
        {
            try
            {
                if (userDto == null || string.IsNullOrWhiteSpace(userDto.email) || string.IsNullOrWhiteSpace(userDto.passwords))
                {
                    return BadRequest("Invalid user data");
                }

                User user = new User
                {
                    user_id = Guid.NewGuid(),
                    email = userDto.email,
                    passwords = BCrypt.Net.BCrypt.HashPassword(userDto.passwords),
                    IsActivated = false
                };

                UserRole userRole = new UserRole
                {
                    user_id = user.user_id,
                    role = userDto.role
                };

                // Create checkout automatically
                Checkout checkout = new Checkout
                {
                    checkout_id = Guid.NewGuid(),
                    user_id = user.user_id,
                    create_date = DateTime.Now
                };

                bool result = _userData.CreateUserAccount(user, userRole, checkout);

                if (result)
                {
                    bool mailResult = await SendEmailActivation(user);
                    return StatusCode(201, userDto);
                }
                else
                {
                    return StatusCode(500, "Data not inserted");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("login")]

        public IActionResult Login([FromBody] LoginRequestDTO credential)
        {
            if (credential is null) return BadRequest("Invalid client request");

            if (string.IsNullOrEmpty(credential.email) || string.IsNullOrEmpty(credential.passwords)) return BadRequest("Invalid client request");

            User? user = _userData.CheckUserAuth(credential.email);

            if (user == null) return Unauthorized("You do not authorized");

            if (!user.IsActivated)
            {
                return Unauthorized("Please activate your account");
            }

            UserRole? userRole = _userData.GetUserRole(user.user_id);

            bool isVerified = BCrypt.Net.BCrypt.Verify(credential.passwords, user?.passwords);
            //bool isVerified = user?.Password == credential.Password;

            if (user != null && !isVerified)
            {
                return BadRequest("Inccorrect Password! Please check your password");
            }
            else
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtConfig:Key").Value));

                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.email),
                    new Claim(ClaimTypes.Role, userRole.role),
                    new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
                };

                var signingCredential = new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(1000),
                    SigningCredentials = signingCredential
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var securityToken = tokenHandler.CreateToken(tokenDescriptor);

                string token = tokenHandler.WriteToken(securityToken);

                return Ok(new LoginResponseDTO { Token = token });

            }
        }

        [Authorize]
        [HttpGet("GetUserData")]
        public IActionResult GetUserData()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return Ok(new
                {
                    Id = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value
                });
            }
            return Unauthorized();
        }


        [HttpGet("ActivateUser")]
        public IActionResult ActivateUser(Guid id, string email)
        {
            try
            {
                User? user = _userData.CheckUserAuth(email);

                if (user == null)
                    return BadRequest("Activation Failed");

                if (user.IsActivated == true)
                    return BadRequest("User has been activated");

                bool result = _userData.ActivateUser(id);

                if (result)
                    return Ok("User activated");
                else
                    return StatusCode(500, "Activation Failed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO forgetPassword)
        {
            try
            {
                if (forgetPassword == null || string.IsNullOrEmpty(forgetPassword.Email))
                    return BadRequest("Email is empty");

                bool sendMail = await SendEmailForgetPassword(forgetPassword.Email);

                if (sendMail)
                {
                    return Ok("Mail sent");
                }
                else
                {
                    return StatusCode(500, "Error");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        private async Task<bool> SendEmailForgetPassword(string email)
        {
            // send email
            List<string> to = new List<string>();
            to.Add(email);

            string subject = "Forget Password";

            var param = new Dictionary<string, string?>
            {
                {"email", email }
            };

            string callbackUrl = QueryHelpers.AddQueryString("http://52.237.194.35:2027/new-password", param);

            string body = "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>";

            EmailModel mailModel = new EmailModel(to, subject, body);

            bool mailResult = await _mail.SendAsync(mailModel, new CancellationToken());

            return mailResult;
        }

        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            try
            {
                if (resetPassword == null)
                    return BadRequest("No Data");

                if (resetPassword.Password != resetPassword.ConfirmPassword)
                {
                    return BadRequest("Password doesn't match");
                }

                bool reset = _userData.ResetPassword(resetPassword.Email, BCrypt.Net.BCrypt.HashPassword(resetPassword.Password));

                if (reset)
                {
                    return Ok("Reset password OK");
                }
                else
                {
                    return StatusCode(500, "Error");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<bool> SendEmailActivation(User user)
        {
            if (user == null)
                return false;

            if (string.IsNullOrEmpty(user.email)) // Ubah dari user.Email menjadi user.email
                return false;

            // send email
            List<string> to = new List<string>();
            to.Add(user.email);

            string subject = "Account Activation";

            var param = new Dictionary<string, string?>
    {
        {"id", user.user_id.ToString() }, // Ubah dari user.Id.ToString() menjadi user.user_id.ToString()
        {"email", user.email } // Tambahkan email sebagai parameter
    };

            string callbackUrl = QueryHelpers.AddQueryString("http://52.237.194.35:2028/api/User/ActivateUser", param);

            EmailActivationModel model = new EmailActivationModel()
            {
                Email = user.email,
                Link = callbackUrl
            };

            string body = _mail.GetEmailTemplate(model);

            EmailModel mailModel = new EmailModel(to, subject, body);
            bool mailResult = await _mail.SendAsync(mailModel, new CancellationToken());
            return mailResult;
        }

        [HttpPut]
        public IActionResult Put(Guid id, [FromBody] UserDTO userDto)
        {
            string hashPassword;

            if (userDto == null)
            {
                return BadRequest("Data Should be Inputed");
            }

            if (!string.IsNullOrEmpty(userDto.passwords))
            {
                hashPassword = BCrypt.Net.BCrypt.HashPassword(userDto.passwords);
            }
            else
            {
                hashPassword = userDto.passwords;
            }
                User user = new User
            {
                user_id = Guid.NewGuid(),
                email = userDto.email,
                passwords = hashPassword,
                role = userDto.role,
                IsActivated = userDto.IsActivated,

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
