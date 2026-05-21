using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;

namespace GymManagementSystem.Core.Services
{
    public class SubscriptionService
    {
        private readonly List<Subscription> _subscriptions = new();

        public List<Subscription> GetAllSubscriptions()
        {
            return _subscriptions;
        }

        public List<Subscription> GetSubscriptionsByMember(int memberId)
        {
            return _subscriptions.Where(s => s.MemberId == memberId).ToList();
        }

        public Subscription? GetActiveSubscription(int memberId)
        {
            return _subscriptions.FirstOrDefault(s =>
                s.MemberId == memberId &&
                s.Status == SubscriptionStatus.Active &&
                !s.IsExpired);
        }

        public (bool Success, string Message) CreateSubscription(
            Member member,
            MembershipPackage package,
            PaymentMethod paymentMethod)
        {
            // Sağlık raporu kontrolü (UC-03)
            if (member.MedicalReport == null)
                return (false, "Uye saglik raporu bulunmuyor!");

            if (member.MedicalReport.Status == ReportStatus.Expired)
                return (false, "Saglik raporu suresi dolmus! Yeni rapor gerekli.");

            // Paket aktif mi kontrolü (UC-03)
            if (!package.IsActive)
                return (false, "Secilen paket aktif degil!");

            // Zaten aktif abonelik var mı?
            if (GetActiveSubscription(member.Id) != null)
                return (false, "Uyenin zaten aktif bir aboneligi var!");

            // Abonelik oluştur
            var subscription = new Subscription
            {
                Id = _subscriptions.Count > 0 ?
                    _subscriptions.Max(s => s.Id) + 1 : 1,
                MemberId = member.Id,
                MembershipPackageId = package.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(package.DurationMonths),
                Status = SubscriptionStatus.Active,
                Member = member,
                MembershipPackage = package
            };

            _subscriptions.Add(subscription);
            return (true, $"Abonelik basariyla olusturuldu! Bitis: {subscription.EndDate:dd/MM/yyyy}");
        }

        public bool CancelSubscription(int subscriptionId)
        {
            var sub = _subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
            if (sub == null) return false;

            sub.Status = SubscriptionStatus.Cancelled;
            return true;
        }

        public (bool Success, string Message) RenewSubscription(
                int subscriptionId,
                PaymentMethod paymentMethod)
        {
            var sub = _subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
            if (sub == null)
                return (false, "Abonelik bulunamadi!");

            if (sub.Status == SubscriptionStatus.Active && !sub.IsExpired)
                return (false, "Abonelik hala aktif, yenileme yapılamaz!");

            // Sağlık raporu kontrolü
            if (sub.Member?.MedicalReport == null ||
                sub.Member.MedicalReport.Status == ReportStatus.Expired)
                return (false, "Saglik raporu gecersiz! Yenileme yapilamaz.");

            // Aboneliği yenile
            sub.StartDate = DateTime.Today;
            sub.EndDate = DateTime.Today.AddMonths(
                sub.MembershipPackage?.DurationMonths ?? 1);
            sub.Status = SubscriptionStatus.Active;

            return (true, $"Abonelik yenilendi! Yeni bitis: {sub.EndDate:dd/MM/yyyy}");
        }
        public void LoadSubscription(Subscription subscription)
        {
            if (!_subscriptions.Any(s => s.Id == subscription.Id))
                _subscriptions.Add(subscription);
        }
    }
}