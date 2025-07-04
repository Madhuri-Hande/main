using System.Diagnostics;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;

namespace Expense_Tracker.Controllers
{
    [ApiController]
    [Route("api/HomeController")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
     
        public IActionResult Test()
        {
            return Ok("Swagger is working!");
        }
    }
}
