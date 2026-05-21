using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;
using GymManagementSystem.Core.Services;

namespace GymManagementSystem.Tests
{
    [TestClass]
    public class NotificationServiceTests
    {
        private MemberService _memberService = new();
        private NotificationService _notificationService = new(new MemberService());

        [TestInitialize]
        public void Setup()
        {
            _memberService = new MemberService();
            _notificationService = new NotificationService(_memberService);
        }

        [TestMethod]
        public void GetExpiredReportNotifications_ReturnsCorrectCount()
        {
            // Arrange
            _memberService.AddMember(new Member
            {
                NationalId = "12345678901",
                FirstName = "Ali",
                LastName = "Yılmaz",
                MedicalReport = new MedicalReport
                {
                    ReportDate = DateTime.Today.AddYears(-2) // Geçersiz rapor
                }
            });

            // Act
            var result = _notificationService.GetExpiredReportNotifications();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(NotificationType.ReportExpired, result[0].Type);
        }

        [TestMethod]
        public void GetExpiringSoonNotifications_ReturnsCorrectCount()
        {
            // Arrange
            _memberService.AddMember(new Member
            {
                NationalId = "12345678901",
                FirstName = "Ayşe",
                LastName = "Kaya",
                MedicalReport = new MedicalReport
                {
                    ReportDate = DateTime.Today.AddYears(-1).AddDays(20) // 10 gün kaldı
                }
            });

            // Act
            var result = _notificationService.GetExpiringSoonNotifications();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(NotificationType.ReportExpiringSoon, result[0].Type);
        }

        [TestMethod]
        public void GetAllNotifications_ReturnsBothTypes()
        {
            // Arrange
            _memberService.AddMember(new Member
            {
                NationalId = "12345678901",
                FirstName = "Ali",
                LastName = "Yılmaz",
                MedicalReport = new MedicalReport
                {
                    ReportDate = DateTime.Today.AddYears(-2) // Geçersiz
                }
            });

            _memberService.AddMember(new Member
            {
                NationalId = "98765432101",
                FirstName = "Ayşe",
                LastName = "Kaya",
                MedicalReport = new MedicalReport
                {
                    ReportDate = DateTime.Today.AddYears(-1).AddDays(20) // Yakında dolacak
                }
            });

            // Act
            var result = _notificationService.GetAllNotifications();

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetExpiredReportNotifications_NoExpiredReports_ReturnsEmpty()
        {
            // Arrange
            _memberService.AddMember(new Member
            {
                NationalId = "12345678901",
                FirstName = "Ali",
                LastName = "Yılmaz",
                MedicalReport = new MedicalReport
                {
                    ReportDate = DateTime.Today // Geçerli rapor
                }
            });

            // Act
            var result = _notificationService.GetExpiredReportNotifications();

            // Assert
            Assert.AreEqual(0, result.Count);
        }
    }
}