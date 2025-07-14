namespace BudgetManager.Models
{
    public class User
    {
        public int UserId {  get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
    }
}
