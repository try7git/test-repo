using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;

        // Rol kontrolleri
        public bool IsManager => Role == UserRole.Manager;
        public bool IsReceptionist => Role == UserRole.Receptionist;
        public bool IsMaintenanceStaff => Role == UserRole.MaintenanceStaff;
    }
}