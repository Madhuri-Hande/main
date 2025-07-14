using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Expense_Tracker.Data;
using Expense_Tracker.Dtos;
using Expense_Tracker.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Authorization;

namespace Expense_Tracker.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoryController> _logger;
        private readonly IConfiguration _configuration;

        public CategoryController(ILogger<CategoryController> logger, AppDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var category = new ExpenseCategory
            {
                ExpenseType = dto.ExpenseType,
                ExpenseDescription = dto.ExpenseDescription,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow
            };

            _context.ExpenseCategories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        [HttpGet("GetCategory")]
        public IActionResult GetExpenses()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var category = _context.ExpenseCategories
                .Where(e => e.CreatedBy == userId)
                .OrderByDescending(e => e.CreatedDate)
                .ToList();

            return Ok(category);
        }

    }
}
