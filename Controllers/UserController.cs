using E_CommerceSystem.Models;
using E_CommerceSystem.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using E_CommerceSystem.DTOs;

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
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("User data is required.");

                _userService.Register(dto);
                return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while registering the user. {ex.Message}");
            }
        }

        // ✅ Refresh token
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshRequestDTO dto)
        {
            var user = _userService.GetUserByRefreshToken(dto.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            // Generate a new JWT
            var newJwt = GenerateJwtToken(user);

            // Generate a new refresh token & save
            var newRefresh = Guid.NewGuid().ToString();
            _userService.SaveRefreshToken(user, newRefresh);

            return Ok(new AuthResponseDTO
            {
                Token = newJwt,
                RefreshToken = newRefresh,
                Expiration = DateTime.UtcNow.AddMinutes(15)
            });
        }

        // ✅ Login
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            try
            {
                var user = _userService.Login(dto);
                if (user == null) return Unauthorized("Invalid email or password.");

                var token = GenerateJwtToken(user);

                return Ok(new AuthResponseDTO
                {
                    Token = token,
                    RefreshToken = user.RefreshToken!,
                    Expiration = DateTime.UtcNow.AddMinutes(15)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while logging in. {ex.Message}");
            }
        }

        // ✅ Get user by ID
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

        // 🔑 Helper: generate JWT token directly from User
        [NonAction]
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UID.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UName),
                new Claim("role", user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
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
