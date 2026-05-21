using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;

namespace GymManagementSystem.Core.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // Cihaz adı
        public string Brand { get; set; } = string.Empty;       // Marka
        public string Model { get; set; } = string.Empty;       // Model
        public DateTime PurchaseDate { get; set; }              // Alım tarihi
        public decimal PurchaseCost { get; set; }               // Alım maliyeti
        public DeviceStatus Status { get; set; } = DeviceStatus.Active;
        public string GeneralMaintenanceStatus { get; set; } = string.Empty;

        // İlişkiler
        public List<MaintenanceRecord> MaintenanceRecords { get; set; } = new();
        public List<RepairRecord> RepairRecords { get; set; } = new();

        // Hesaplanan özellik
        public decimal TotalMaintenanceCost =>
            MaintenanceRecords.Sum(m => m.Cost) + RepairRecords.Sum(r => r.Cost);
    }

    public enum DeviceStatus
    {
        Active,            // Aktif
        UnderMaintenance,  // Bakımda (salon içi)
        UnderRepair,       // Tamirde (teknik servis)
        Passive            // Pasif
    }
}
