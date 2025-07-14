using System.ComponentModel.DataAnnotations;

namespace BudgetManager.DTOs.ExpensesDTOs
{
    public class ExpenseUpdateDto
    {
        [Required]
        public int ExpenseID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        public string? Comment { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}
