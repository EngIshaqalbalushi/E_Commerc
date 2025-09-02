using E_CommerceSystem.Models;
using E_CommerceSystem.DTOs;
namespace E_CommerceSystem.Repositories
{
    public class UserRepo : IUserRepo
    {
        public ApplicationDbContext _context;
        public UserRepo(ApplicationDbContext context)
        {
            _context = context;
        }


        public User? GetByEmail(string email)
{
    return _context.Users.FirstOrDefault(u => u.Email == email);
}


        public User? GetByRefreshToken(string refreshToken)
        {
            return _context.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        }

        //Get All users
        public IEnumerable<User> GetAllUsers()
        {
            try
            {
                return _context.Users.ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }

        //Get user by id
        public User GetUserById(int uid)
        {
            try
            {
                return _context.Users.FirstOrDefault(u => u.UID == uid);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }
        //Add new user
        public void AddUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }


        //Update User 
        public void UpdateUser(User user)
        {
            try
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.UID == user.UID);
                if (existingUser == null)
                    throw new KeyNotFoundException($"User with ID {user.UID} not found.");

                existingUser.UName = user.UName;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.Role = user.Role;

                // Update password only if changed
                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    existingUser.PasswordHash = user.PasswordHash;
                }

                _context.Users.Update(existingUser);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }


        //Delete User
        public void DeleteUser(int uid)
        {
            try
            {
                var user = GetUserById(uid);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }

        //Get user by email and passward
        public User? GetUSer(string email, string password)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);

                // Compare provided password with the hashed password in DB
                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    return user;
                }

                return null;  // Invalid credentials
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error: {ex.Message}");
            }
        }

    }
}
