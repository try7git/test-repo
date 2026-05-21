using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Core.Entities
{
    public enum UserRole
    {
        Member,           // Üye
        Receptionist,     // Resepsiyonist
        Manager,          // Yönetici
        MaintenanceStaff, // Bakım Görevlisi
        Trainer           // Antrenör
    }
}
