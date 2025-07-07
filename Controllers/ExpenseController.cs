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
    public class ExpenseController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExpenseController> _logger;
        private readonly IConfiguration _configuration;

        public ExpenseController(ILogger<ExpenseController> logger, AppDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("CreateExpences")]
        public async Task<IActionResult> CreateExpenses([FromBody] ExpensesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the current user's ID from the claims
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var expense = new Expense
            {
                Category = dto.Category,
                Amount = dto.Amount,
                Description = dto.Description,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            // Save to database (assuming _context is your DbContext)
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return Ok(expense);
        }

    }
}
