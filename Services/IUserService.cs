using E_CommerceSystem.DTOs;
using E_CommerceSystem.Models;

namespace E_CommerceSystem.Services
{
    public interface IUserService
    {
        // 🔑 Register a new user
        void Register(RegisterDTO dto);

        // 🔑 Login with DTO
        User Login(LoginDTO dto);

        // 🔑 CRUD
        IEnumerable<User> GetAllUsers();
        User GetUserById(int uid);
        void UpdateUser(User user);
        void DeleteUser(int uid);

        // 🔑 Refresh token support
        User? GetUserByRefreshToken(string refreshToken);
        void SaveRefreshToken(User user, string newToken);
    }
}
