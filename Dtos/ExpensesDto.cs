using Expense_Tracker.Models;

namespace Expense_Tracker.Dtos
{
    public class ExpensesDto
    {
        //public int ExpenseId { get; set; }

        public ExpenseCategory? Category { get; set; }

        public decimal Amount { get; set; }

        public string? Description { get; set; }

     //   public int CreatedBy { get; set; }

      //  public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

     //   public int? UpdatedBy { get; set; }

//        public bool IsDeleted { get; set; } = false;
    }
}
