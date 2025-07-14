using System.ComponentModel.DataAnnotations;

namespace BudgetManager.DTOs.UserDTOs
{
    public class UserRegisterDto
    {
        [Required]
        public string Username {  get; set; } = string.Empty;

        [Required]
        [MinLength(7, ErrorMessage = "Password must contain at least 7 characters")]
        public string Password { get; set; } = string.Empty;
    }
}
