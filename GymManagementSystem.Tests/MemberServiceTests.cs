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
    public class MemberServiceTests
    {
        private MemberService _memberService = new();

        [TestInitialize]
        public void Setup()
        {
            _memberService = new MemberService();
        }

        [TestMethod]
        public void AddMember_ValidMember_ReturnsTrue()
        {
            // Arrange
            var member = new Member
            {
                FirstName = "Ali",
                LastName = "Yılmaz",
                NationalId = "12345678901",
                BirthDate = new DateTime(1990, 1, 1),
                Phone = "05001234567",
                Email = "ali@test.com"
            };

            // Act
            var result = _memberService.AddMember(member);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddMember_DuplicateNationalId_ReturnsFalse()
        {
            // Arrange
            var member1 = new Member { NationalId = "12345678901", FirstName = "Ali" };
            var member2 = new Member { NationalId = "12345678901", FirstName = "Veli" };

            // Act
            _memberService.AddMember(member1);
            var result = _memberService.AddMember(member2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetById_ExistingMember_ReturnsMember()
        {
            // Arrange
            var member = new Member { NationalId = "12345678901", FirstName = "Ali" };
            _memberService.AddMember(member);

            // Act
            var result = _memberService.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Ali", result.FirstName);
        }

        [TestMethod]
        public void DeleteMember_ExistingMember_ReturnsTrue()
        {
            // Arrange
            var member = new Member { NationalId = "12345678901", FirstName = "Ali" };
            _memberService.AddMember(member);

            // Act
            var result = _memberService.DeleteMember(1);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, _memberService.GetAllMembers().Count);
        }

        [TestMethod]
        public void GetMembersWithExpiredReports_ReturnsCorrectMembers()
        {
            // Arrange
            var member = new Member
            {
                NationalId = "12345678901",
                FirstName = "Ali",
                MedicalReport = new MedicalReport
                {
                    ReportDate = DateTime.Today.AddYears(-2) // 2 yıl önce = geçersiz
                }
            };
            _memberService.AddMember(member);

            // Act
            var result = _memberService.GetMembersWithExpiredReports();

            // Assert
            Assert.AreEqual(1, result.Count);
        }
    }
}