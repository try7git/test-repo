using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Core.Entities
{
    public class MaintenanceRecord
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string Description { get; set; } = string.Empty;  // Açıklama
        public string AssignedStaff { get; set; } = string.Empty; // Görevli
        public decimal Cost { get; set; }
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Completed;

        // Bakım her zaman spor salonunda yapılır (PDF'den)
        public string Location { get; set; } = "Spor Salonu";

        // İlişki
        public Device? Device { get; set; }
    }

    public enum MaintenanceStatus
    {
        Planned,   // Planlandı
        Completed  // Tamamlandı
    }
}