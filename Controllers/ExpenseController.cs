using Microsoft.AspNetCore.Mvc;
using Expense_Tracker.Data;
using Expense_Tracker.Dtos;
using Expense_Tracker.Models;
using System.Security.Claims;
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
        public ExpenseController(ILogger<ExpenseController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : (int?)null;
        }

        [HttpPost("CreateExpences")]
        public async Task<IActionResult> CreateExpenses([FromBody] ExpensesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized("User ID not found in token.");
            try
            {
                var expense = new Expense
                {
                    ExpenseCategoryId = dto.ExpenseCategoryId,
                    Description = dto.Description,
                    Amount = dto.Amount,
                    CreatedBy = (int)userId,
                    CreatedDate = DateTime.UtcNow,
                };

                _context.Expenses.Add(expense);
                await _context.SaveChangesAsync();

                return Ok(expense);
            }
            catch (Exception ex) {

                _logger.LogError(ex, "Error creating expense.");
                return StatusCode(500, "An error occurred while creating the expense.");

            }
        }

        [HttpGet("GetExpenses")]
        public IActionResult GetExpenses()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized("User ID not found in token.");
            try
            {
                var expenses = _context.Expenses
                    .Where(e => e.CreatedBy == userId)
                    .OrderByDescending(e => e.CreatedDate)
                    .ToList();

                return Ok(expenses);
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error retrieving expenses.");
                return StatusCode(500, "An error occurred while retrieving expenses.");
            }
        }

        [HttpPut("UpdateExpense/{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] ExpensesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized("User ID not found in token.");
            try
            {
                var expense = await _context.Expenses.FindAsync(id);
                if (expense == null || expense.CreatedBy != userId)
                    return NotFound();

                expense.ExpenseCategoryId = dto.ExpenseCategoryId;
                expense.Amount = dto.Amount;
                expense.Description = dto.Description;
                expense.UpdatedBy = userId;
                // expense.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(expense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating expense with ID {id}.");
                return StatusCode(500, "An error occurred while updating the expense.");
            }
        }

        [HttpDelete("DeleteExpense/{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized("User ID not found in token.");
            try
            {
                var expense = await _context.Expenses.FindAsync(id);
                if (expense == null || expense.CreatedBy != userId)
                    return NotFound();

                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Expense deleted successfully." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting expense with ID {id}.");
                return StatusCode(500, "An error occurred while deleting the expense.");

            }
        }
    }
}
