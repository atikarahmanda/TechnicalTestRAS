using FinalProjectRAS.Models;
using FinalProjectRAS.Repositories;
using FinalProjectRAS.Utils;
using FinalProjectRAS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinalProjectRAS.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;
        public UserController(UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        //[Authorize(Roles = "Admin")]
        public IActionResult AddUser(RegisterVM registerVM)
        {
            var addUser = _userRepository.Register(registerVM);
            if (addUser == -2)
            {
                return BadRequest(new { status = 400, message = $"Failed to send Email!!" });
            }
            if (addUser > 0)
            {
                return Ok(new { status = 200, message = $"Data has been Added!" });
            }
            else if (addUser < 0)
            {
                return BadRequest(new { status = 400, message = $"Email is Registered!" });
            }
            else
            {
                return BadRequest(new { status = 404, message = $"Please fill all fields !" });
            }
        }


        [HttpPost("login")]
        public IActionResult Login(LoginVM account)
        {
            var (loginResult, userData) = _userRepository.Login(account);
            if (loginResult == -1)
            {
                return NotFound(new { status = 404, message = "Email Not Found!" });
            }
            else if (loginResult == 0)
            {
                return BadRequest(new { status = 404, message = "Wrong Password" });
            }
            string? isCompleted = userData.IsCompleted.ToString();
            string? isDeadline = userData.Deadline.ToString();

            var claims = new List<Claim>
            {
                new Claim("userId", userData.UserId.ToString()),
                new Claim("Email", userData.Email),
                new Claim("Fullname", userData.Fullname),
                new Claim("IsCompleted", isCompleted),
                new Claim("Deadline", isDeadline)
            };
            claims.Add(new Claim(ClaimTypes.Role, userData.Role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:API"],
            claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: signIn
            );

            var tokenResult = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { status = 200, message = "Login Successful", data = tokenResult });
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCandidateData()
        {
            var results = _userRepository.GetCandidateData();
            var response = new
            {
                Status = "200",
                Message = "Data retrieved successfully",
                Data = results
            };
            return Ok(response);
        }

        [HttpGet("PastDeadline")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetCandidatePastDeadline()
        {
            var results = _userRepository.GetCandidatePastDeadline();
            var response = new
            {
                Status = "200",
                Message = "Data retrieved successfully",
                Data = results
            };
            return Ok(response);
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUserById(Guid userId)
        {
            var user = _userRepository.GetUserById(userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(user);
        }

        [HttpPut("UpdateDeadline/{userId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateDeadline(Guid userId, [FromBody] int? newDeadlineDays)
        {
            if (newDeadlineDays == null)
            {
                var response = new
                {
                    Status = "400",
                    Message = "Deadline days are required.",
                };
                return BadRequest(response);
            }

            var result = _userRepository.UpdateDeadline(userId, newDeadlineDays.Value);
            if (result == 0)
            {
                var response = new
                {
                    Status = "400",
                    Message = "User not found.",
                };
                return NotFound(response);
            }
            else
            {
                var user = _userRepository.GetUserById(userId);
                var response = new
                {
                    Status = "200",
                    Message = "Deadline updated successfully",
                    Data = user.Deadline?.ToString("yyyy-MM-dd")
                };
                return Ok(response);
            }
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(Guid userId)
        {
            var result = _userRepository.DeleteUser(userId);

            if (result == 0)
            {
                var response = new
                {
                    Status = "400",
                    Message = "User not found.",
                };
                return NotFound(response);
            }
            else
            {
                var response = new
                {
                    Status = "200",
                    Message = "User deleted successfully."
                };
                return Ok(response);
            }

        }

        [HttpPut("UpdateRequest/{userId}")]
        [Authorize(Roles = "Admin, Candidate")]
        public IActionResult UpdateRequestStatus(Guid userId, [FromBody] bool statusTest)
        {
            bool isUpdated = _userRepository.UpdateRequestStatus(userId, statusTest);

            if (!isUpdated)
            {
                return NotFound(new { message = "User request not found" });
            }

            return Ok(new { message = "Request updated successfully" });
        }

        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public ActionResult<StatisticVM> GetStatisticCandidate()
        {
            try
            {
                var statistics = _userRepository.GetStatisticCandidate();
                var response = new
                {
                    Status = "200",
                    Message = "Data retrieved successfully",
                    Data = statistics
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
    }
}
