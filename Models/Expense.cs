namespace BudgetManager.Models
{
    public class Expense
    {
        public int ExpenseID { set; get; }
        public int CategoryID { set; get; }
        public Category? Category { set; get; }
        public decimal Price { set; get; }
        public string? Comment { set; get; }
        public DateTime Date { set; get; }
    }
}