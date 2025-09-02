using E_CommerceSystem.DTOs;
using E_CommerceSystem.Models;
using E_CommerceSystem.Repositories;
using System.Security.Cryptography;

namespace E_CommerceSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
        }

        // ✅ Register
        public void Register(RegisterDTO dto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                UName = dto.UName,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = hashedPassword,
                Role = string.IsNullOrEmpty(dto.Role) ? "Customer" : dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            _userRepo.AddUser(user);
        }

        // ✅ Login
        public User Login(LoginDTO dto)
        {
            var user = _userRepo.GetByEmail(dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return user;
        }

        // ✅ Get all users
        public IEnumerable<User> GetAllUsers()
        {
            return _userRepo.GetAllUsers();
        }

        // ✅ Get user by ID
        public User GetUserById(int uid)
        {
            var user = _userRepo.GetUserById(uid);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {uid} not found.");
            return user;
        }

        // ✅ Update user
        public void UpdateUser(User user)
        {
            var existingUser = _userRepo.GetUserById(user.UID);
            if (existingUser == null)
                throw new KeyNotFoundException($"User with ID {user.UID} not found.");

            _userRepo.UpdateUser(user);
        }

        // ✅ Delete user
        public void DeleteUser(int uid)
        {
            var user = _userRepo.GetUserById(uid);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {uid} not found.");

            _userRepo.DeleteUser(uid);
        }

        // ✅ Refresh token: get by token
        public User? GetUserByRefreshToken(string refreshToken)
        {
            return _userRepo.GetByRefreshToken(refreshToken);
        }

        // ✅ Refresh token: save new token
        public void SaveRefreshToken(User user, string newToken)
        {
            user.RefreshToken = newToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _userRepo.UpdateUser(user);
        }
    }
}
