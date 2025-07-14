using System.ComponentModel.DataAnnotations;

namespace BudgetManager.DTOs.CategoryDTOs
{
    public class CategoryCreateDto
    {
        [Required]
        public string Name { set; get; } = string.Empty;
    }
}
