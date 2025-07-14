namespace BudgetManager.DTOs.ExpensesDTOs
{
    public class ExpenseReadDto
    {
        public int ExpenseID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Comment { get; set; }
        public DateTime Date { get; set; }
    }
}
