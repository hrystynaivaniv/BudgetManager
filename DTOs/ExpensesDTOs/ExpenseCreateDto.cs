using System.ComponentModel.DataAnnotations;

namespace BudgetManager.DTOs.ExpensesDTOs
{
    public class ExpenseCreateDto
    {
            [Required]
            public int CategoryID { get; set; }

            [Required]
            [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
            public decimal Price { get; set; }

            public string? Comment { get; set; }

            [Required]
            public DateTime Date { get; set; }
    }
}
