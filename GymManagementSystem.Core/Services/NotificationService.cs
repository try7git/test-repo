using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;

namespace GymManagementSystem.Core.Services
{
    public class Notification
    {
        public string MemberName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public enum NotificationType
    {
        ReportExpired,     // Rapor süresi doldu
        ReportExpiringSoon // Rapor yakında dolacak
    }

    public class NotificationService
    {
        private readonly MemberService _memberService;

        public NotificationService(MemberService memberService)
        {
            _memberService = memberService;
        }

        // Süresi dolan raporlar için bildirim (PDF UC-07)
        public List<Notification> GetExpiredReportNotifications()
        {
            return _memberService.GetMembersWithExpiredReports()
                .Select(m => new Notification
                {
                    MemberName = m.FullName,
                    Message = $"{m.FullName} adlı üyenin sağlık raporu süresi dolmuştur.",
                    Type = NotificationType.ReportExpired
                }).ToList();
        }

        // Yakında dolacak raporlar için bildirim (PDF UC-07)
        public List<Notification> GetExpiringSoonNotifications()
        {
            return _memberService.GetMembersWithExpiringSoonReports()
                .Select(m => new Notification
                {
                    MemberName = m.FullName,
                    Message = $"{m.FullName} adlı üyenin sağlık raporu 30 gün içinde dolacaktır.",
                    Type = NotificationType.ReportExpiringSoon
                }).ToList();
        }

        // Tüm bildirimler
        public List<Notification> GetAllNotifications()
        {
            var notifications = new List<Notification>();
            notifications.AddRange(GetExpiredReportNotifications());
            notifications.AddRange(GetExpiringSoonNotifications());
            return notifications;
        }
    }
}
