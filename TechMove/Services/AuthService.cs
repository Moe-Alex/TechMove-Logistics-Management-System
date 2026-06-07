using TechMove.Models;
using TechMove.Models.Enums;

namespace TechMove.Services
{
    public class AuthService
    {
        private readonly List<AppUser> _users = new()
        {
            new AppUser { Username = "Admin", Password = "Admin1", Role = UserRole.Admin },
            new AppUser { Username = "User", Password = "User1", Role = UserRole.User }
        };

        public AppUser? ValidateUser(string username, string password)
        {
            return _users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);
        }
    }
}