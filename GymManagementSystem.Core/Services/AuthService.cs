using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;

namespace GymManagementSystem.Core.Services
{
    public class AuthService
    {
        private readonly List<User> _users = new();
        private User? _currentUser;

        public AuthService()
        {
            // Varsayılan yönetici hesabı
            _users.Add(new User
            {
                Id = 1,
                Username = "admin",
                Password = "admin123",
                FullName = "Sistem Yöneticisi",
                Role = UserRole.Manager,
                IsActive = true
            });
        }

        public User? CurrentUser => _currentUser;
        public bool IsLoggedIn => _currentUser != null;

        // Giriş işlemi
        public bool Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == password &&
                u.IsActive);

            if (user == null) return false;

            _currentUser = user;
            return true;
        }

        // Çıkış işlemi
        public void Logout()
        {
            _currentUser = null;
        }

        // Rol kontrolü (PDF FR-13)
        public bool HasPermission(UserRole requiredRole)
        {
            if (_currentUser == null) return false;
            return _currentUser.Role == requiredRole ||
                   _currentUser.Role == UserRole.Manager;
        }

        // Kullanıcı yönetimi
        public void AddUser(User user)
        {
            user.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
            _users.Add(user);
        }

        public List<User> GetAllUsers()
        {
            return _users;
        }

        public bool UpdateUser(User updated)
        {
            var existing = _users.FirstOrDefault(u => u.Id == updated.Id);
            if (existing == null) return false;

            existing.FullName = updated.FullName;
            existing.Role = updated.Role;
            existing.IsActive = updated.IsActive;
            return true;
        }
    }
}
