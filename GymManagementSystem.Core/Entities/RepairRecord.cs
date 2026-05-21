using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Core.Entities
{
    public class RepairRecord
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public DateTime RepairDate { get; set; }
        public string Description { get; set; } = string.Empty;   // Açıklama
        public string ServiceCompany { get; set; } = string.Empty; // Teknik servis bilgisi
        public decimal Cost { get; set; }
        public RepairStatus Status { get; set; } = RepairStatus.Completed;

        // Tamir her zaman teknik serviste yapılır (PDF'den)
        public string Location { get; set; } = "Teknik Servis";

        // İlişki
        public Device? Device { get; set; }
    }

    public enum RepairStatus
    {
        InProgress, // Devam Ediyor
        Completed   // Tamamlandı
    }
}
