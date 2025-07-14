using System.ComponentModel.DataAnnotations;

namespace BudgetManager.DTOs.CategoryDTOs
{
    public class CategoryUpdateDto
    {
        [Required]
        public int CategoryID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}