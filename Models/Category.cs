namespace BudgetManager.Models
{
    public class Category
    {
        public int CategoryID { set; get; }
        public string Name { set; get; } = string.Empty;
        public ICollection<Expense> Expenses { set; get; } = new List<Expense>();
    }
}