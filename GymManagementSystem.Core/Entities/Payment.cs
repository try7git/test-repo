using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Completed;
        public string Aciklama { get; set; } = string.Empty; // Makbuz/açıklama

        // İlişkiler
        public Member? Member { get; set; }
        public Subscription? Subscription { get; set; }
    }

    public enum PaymentMethod
    {
        Cash,       // Nakit
        CreditCard  // Kredi Kartı
    }

    public enum PaymentStatus
    {
        Completed, // Tamamlandı
        Pending,   // Beklemede
        Partial,   // Kısmi Ödeme
        Failed,    // Başarısız
        Cancelled  // İptal
    }
}