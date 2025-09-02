using E_CommerceSystem.Models;
using E_CommerceSystem.Models.DTOs;
using E_CommerceSystem.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_CommerceSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IConfiguration configuration, IMapper mapper)
        {
            _userService = userService;
            _configuration = configuration;
            _mapper = mapper;
        }

        // ✅ Register new user
        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserDTO inputUser)
        {
            try
            {
                if (inputUser == null)
                    return BadRequest("User data is required.");

                var user = _mapper.Map<User>(inputUser);
                user.CreatedAt = DateTime.Now;

                _userService.AddUser(user);

                return Ok(_mapper.Map<UserDTO>(user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the user. {ex.Message}");
            }
        }

        // ✅ Login user → return JWT token
        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Login(string email, string password)
        {
            try
            {
                var user = _userService.GetUSer(email, password);
                if (user == null) return Unauthorized("Invalid email or password.");

                string token = GenerateJwtToken(user.UID.ToString(), user.UName, user.Role);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while login. {ex.Message}");
            }
        }

        // ✅ Get user by ID (returns DTO, not entity)
        [HttpGet("GetUserById/{UserID}")]
        public IActionResult GetUserById(int UserID)
        {
            try
            {
                var user = _userService.GetUserById(UserID);
                if (user == null) return NotFound();

                var userDto = _mapper.Map<UserDTO>(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving user. {ex.Message}");
            }
        }

        // 🔑 Helper: generate JWT token
        [NonAction]
        public string GenerateJwtToken(string userId, string username, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Name, username),
                new Claim("role", role), // explicit role claim
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
