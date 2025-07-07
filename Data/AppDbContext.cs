using Expense_Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Expense> Expenses { get; set; }

        //public DbSet<ExpenseCategory> Categories { get; set; }
    }

}
