using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Expense_Tracker.Data;
using Expense_Tracker.Dtos;
using Expense_Tracker.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Text;



namespace Expense_Tracker.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class ApplicationUserController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ApplicationUserController> _logger;
        private readonly IConfiguration _configuration;


        public ApplicationUserController(ILogger<ApplicationUserController> logger, AppDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (_context.Users.Any(u => u.Username == dto.Username))
                return BadRequest("Username already exists");


            var user = new ApplicationUser
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = dto.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }



        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || user.Password != dto.Password) 
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

    }
}
