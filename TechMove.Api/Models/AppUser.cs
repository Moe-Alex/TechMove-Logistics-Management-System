using TechMove.Models.Enums;

namespace TechMove.Models
{
    public class AppUser
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}