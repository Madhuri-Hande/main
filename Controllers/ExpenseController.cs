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

        [HttpPost("Create Expences")]
        public async Task<IActionResult> CreateExpenses([FromBody] ExpensesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return Ok(expense);
        }

        [HttpGet("GetExpenses")]
        public IActionResult GetExpenses()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var expenses = _context.Expenses
                .Where(e => e.CreatedBy == userId)
                .OrderByDescending(e => e.CreatedDate)
                .ToList();

            return Ok(expenses);
        }

        [HttpPut("UpdateExpense/{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] ExpensesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null || expense.CreatedBy != userId)
                return NotFound();

            expense.Category = dto.Category;
            expense.Amount = dto.Amount;
            expense.Description = dto.Description;
            expense.UpdatedBy = userId;
            // expense.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(expense);
        }

        [HttpDelete("DeleteExpense/{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null || expense.CreatedBy != userId)
                return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Expense deleted successfully." });
        }

    }
}
