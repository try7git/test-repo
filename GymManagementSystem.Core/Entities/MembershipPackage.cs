using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Core.Entities
{
    public class MembershipPackage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Paket adı
        public PackageType PackageType { get; set; }      // Haftalık 2, 3 veya 7 gün
        public TimeSpan StartTime { get; set; }           // Kullanım başlangıç saati
        public TimeSpan EndTime { get; set; }             // Kullanım bitiş saati
        public int DurationMonths { get; set; }           // Geçerlilik süresi (ay)
        public decimal Price { get; set; }                // Ücret
        public bool IsActive { get; set; } = true;        // Aktif/Pasif

        // İlişki
        public List<Subscription> Subscriptions { get; set; } = new();

        // Hesaplanan özellik
        public string TimeRange => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }

    public enum PackageType
    {
        TwoDaysPerWeek,   // Haftada 2 gün
        ThreeDaysPerWeek, // Haftada 3 gün
        EveryDay          // Her gün
    }
}