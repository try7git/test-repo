using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;

namespace GymManagementSystem.Core.Services
{
    public class MemberService
    {
        private readonly List<Member> _members = new();

        // Sadece veri yüklemek için, validasyon olmadan
        public void LoadMember(Member member)
        {
            if (!_members.Any(m => m.NationalId == member.NationalId))
            {
                member.Id = _members.Count > 0 ? _members.Max(m => m.Id) + 1 : 1;
                _members.Add(member);
            }
        }
        public List<Member> GetAllMembers()
        {
            return _members;
        }

        public Member? GetById(int id)
        {
            return _members.FirstOrDefault(m => m.Id == id);
        }

        public Member? GetByNationalId(string nationalId)
        {
            return _members.FirstOrDefault(m => m.NationalId == nationalId);
        }

        public bool AddMember(Member member)
        {
            // Aynı T.C. kimlik no ile kayıt olmamalı (UC-01)
            if (_members.Any(m => m.NationalId == member.NationalId))
                return false;

            // TC kimlik 11 haneli olmalı
            if (member.NationalId.Length != 11)
                return false;

            // TC kimlik sadece rakam olmalı
            if (!member.NationalId.All(char.IsDigit))
                return false;

            // TC kimlik ilk hanesi 0 olamaz
            if (member.NationalId[0] == '0')
                return false;

            // Telefon 10 veya 11 haneli olmalı
            if (!string.IsNullOrEmpty(member.Phone) &&
                (member.Phone.Length < 10 || member.Phone.Length > 11))
                return false;

            // Telefon sadece rakam olmalı
            if (!string.IsNullOrEmpty(member.Phone) &&
                !member.Phone.All(char.IsDigit))
                return false;

            // Rapor 1 yıldan eskiyse kayıt tamamlanmaz (UC-01)
            if (member.MedicalReport != null &&
                member.MedicalReport.Status == ReportStatus.Expired)
                return false;

            member.Id = _members.Count > 0 ? _members.Max(m => m.Id) + 1 : 1;
            _members.Add(member);
            return true;
        }
        public bool UpdateMember(Member updated)
        {
            var existing = GetById(updated.Id);
            if (existing == null) return false;

            existing.FirstName = updated.FirstName;
            existing.LastName = updated.LastName;
            existing.Phone = updated.Phone;
            existing.Email = updated.Email;
            existing.IsActive = updated.IsActive;

            // Rapor güncellemesi
            if (updated.MedicalReport != null)
            {
                if (existing.MedicalReport == null)
                    existing.MedicalReport = new MedicalReport();

                existing.MedicalReport.ReportDate = updated.MedicalReport.ReportDate;
                existing.MedicalReport.IssuedBy = updated.MedicalReport.IssuedBy;
                existing.MedicalReport.MemberId = updated.Id;
            }

            return true;
        }

        public bool DeleteMember(int id)
        {
            var member = GetById(id);
            if (member == null) return false;

            _members.Remove(member);
            return true;
        }

        public List<Member> GetMembersWithExpiredReports()
        {
            return _members.Where(m =>
                m.MedicalReport != null &&
                m.MedicalReport.Status == ReportStatus.Expired)
                .ToList();
        }

        public List<Member> GetMembersWithExpiringSoonReports()
        {
            return _members.Where(m =>
                m.MedicalReport != null &&
                m.MedicalReport.Status == ReportStatus.ExpiringSoon)
                .ToList();
        }
    }
}