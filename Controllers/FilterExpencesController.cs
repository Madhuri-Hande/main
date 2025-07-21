using Microsoft.AspNetCore.Mvc;
using Expense_Tracker.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Expense_Tracker.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class FilterExpencesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FilterExpencesController> _logger;

        public FilterExpencesController(ILogger<FilterExpencesController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : (int?)null;
        }


        [HttpGet("GetTotalExpenses")]
        public IActionResult GetTotalExpenses()
        {
            try
            {
                int? userId = GetCurrentUserId();

                if (userId == null)
                    return Unauthorized("User ID not found in token.");

                decimal total = _context.Expenses
                    .Where(e => e.CreatedBy == userId)
                    .Sum(e => e.Amount);

                return Ok(new { TotalExpenses = total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching total expenses.");
                return StatusCode(500, "An error occurred while processing your request.");

            }
        }



        [HttpGet("GetTotalCategoryWise")]
        public IActionResult GetTotalCategoryWise(int categoryId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User ID not found in token.");

                var total = _context.Expenses
                    .Where(e => e.CreatedBy == userId && e.ExpenseCategoryId == categoryId)
                    .Sum(e => e.Amount);

                return Ok(new { CategoryId = categoryId, Total = total });
            }
            catch (Exception ex) {

                _logger.LogError(ex, $"Error occurred while fetching expenses for category {categoryId}.");
                return StatusCode(500, "An error occurred while processing your request.");

            }
        }


        [HttpGet("GetExpensesCategoryWise")]
        public IActionResult GetExpensesCategoryWise(int categoryId, DateTime? startDate, DateTime? endDate, decimal? minAmount, decimal? maxAmount, string? keyword)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized("User ID not found in token.");

                var query = _context.Expenses
                    .Where(e => e.ExpenseCategoryId == categoryId && e.CreatedBy == userId);

                if (startDate.HasValue)
                    query = query.Where(e => e.CreatedDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(e => e.CreatedDate <= endDate.Value);

                if (minAmount.HasValue)
                    query = query.Where(e => e.Amount >= minAmount.Value);

                if (maxAmount.HasValue)
                    query = query.Where(e => e.Amount <= maxAmount.Value);

                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(e => e.Description.Contains(keyword));

                return Ok(query.ToList());
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error occurred while fetching expenses for category {categoryId}.");
                return StatusCode(500, "An error occurred while processing your request.");

            }
        }
    }
}
