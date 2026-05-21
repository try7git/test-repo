using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Core.Entities
{
    public class BudgetReport
    {
        public int Id { get; set; }
        public int Month { get; set; }   // Ay (1-12)
        public int Year { get; set; }    // Yıl
        public decimal TotalIncome { get; set; }      // Toplam gelir (ödemeler)
        public decimal TotalExpense { get; set; }     // Toplam gider (bakım/tamir)
        public decimal NetBalance => TotalIncome - TotalExpense; // Net bakiye

        // Detay listeleri
        public List<Payment> Payments { get; set; } = new();
        public List<MaintenanceRecord> MaintenanceRecords { get; set; } = new();
        public List<RepairRecord> RepairRecords { get; set; } = new();

        // Hesaplanan özellikler
        public string PeriodLabel => $"{Year}/{Month:D2}";
        public bool IsProfit => NetBalance >= 0;
    }
}
