using System.ComponentModel.DataAnnotations;

namespace BudgetManager.DTOs.CategoryDTOs
{
    public class CategoryReadDto
    {
            public int CategoryID { get; set; }
            public string Name { get; set; } = string.Empty;
    }
}
