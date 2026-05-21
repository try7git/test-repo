using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Core.Entities
{
    public class MedicalReport
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime ExpiryDate => ReportDate.AddYears(1);
        public string IssuedBy { get; set; } = string.Empty; // Doktor/Kurum adı

        // Rapor durumu — PDF'deki 3 durum
        public ReportStatus Status
        {
            get
            {
                var today = DateTime.Today;
                if (ExpiryDate < today)
                    return ReportStatus.Expired;
                else if (ExpiryDate <= today.AddDays(30))
                    return ReportStatus.ExpiringSoon;
                else
                    return ReportStatus.Valid;
            }
        }

        // İlişki
        public Member? Member { get; set; }
    }

    public enum ReportStatus
    {
        Valid,        // Geçerli
        ExpiringSoon, // Yakında Dolacak (30 gün)
        Expired       // Geçersiz
    }
}