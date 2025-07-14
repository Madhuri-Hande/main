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
    public class FilterExpencesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FilterExpencesController> _logger;
        private readonly IConfiguration _configuration;

        public FilterExpencesController(ILogger<FilterExpencesController> logger, AppDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("GetTotalExpenses")]
        public IActionResult GetTotalExpenses()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            decimal total = _context.Expenses
                .Where(e => e.CreatedBy == userId)
                .Sum(e => e.Amount);

            return Ok(total);
        }


        [HttpGet("GetTotalCategoryWise")]
        public IActionResult GetTotalCategoryWise(int categoryId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

            decimal total = _context.Expenses
                .Where(e => e.ExpenseCategoryId == categoryId)
                .Sum(e => e.Amount);

            return Ok(total);
        }

        [HttpGet("GetExpensesCategoryWise")]
        public IActionResult GetExpensesCategoryWise(int categoryId, DateTime? startDate, DateTime? endDate, decimal? minAmount, decimal? maxAmount, string? keyword)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim);

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

    }
}
