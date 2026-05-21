using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Core.Entities
{
    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty; // T.C. Kimlik No
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // İlişkiler
        public MedicalReport? MedicalReport { get; set; }
        public List<Subscription> Subscriptions { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();

        // Hesaplanan özellik
        public string FullName => $"{FirstName} {LastName}";
    }
}
