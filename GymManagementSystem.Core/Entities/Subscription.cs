using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Core.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int MembershipPackageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

        // İlişkiler
        public Member? Member { get; set; }
        public MembershipPackage? MembershipPackage { get; set; }
        public List<Payment> Payments { get; set; } = new();

        // Hesaplanan özellik
        public bool IsExpired => DateTime.Today > EndDate;
    }

    public enum SubscriptionStatus
    {
        Active,    // Aktif
        Expired,   // Süresi Dolmuş
        Cancelled  // İptal Edilmiş
    }
}