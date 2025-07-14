namespace Expense_Tracker.Models
{
    public class ExpenseCategory
    {
        public int ExpenseCategoryId { get; set; }

        public string ExpenseType { get; set; } = string.Empty;

        public string ExpenseDescription { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int UpdateBy { get; set; }

        public DateTime UpdatedDate { get; set; }
        public Expense Expense { get; set; } = null!;
    }
}
