using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;
using Newtonsoft.Json;

namespace GymManagementSystem.Data
{
    public class JsonDataStore
    {
        private readonly string _dataFolder;

        public List<Member> Members { get; set; } = new();
        public List<MembershipPackage> Packages { get; set; } = new();
        public List<Subscription> Subscriptions { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
        public List<Device> Devices { get; set; } = new();
        public List<MaintenanceRecord> MaintenanceRecords { get; set; } = new();
        public List<RepairRecord> RepairRecords { get; set; } = new();
        public List<User> Users { get; set; } = new();

        public JsonDataStore()
        {
            _dataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GymManagementSystem");

            Directory.CreateDirectory(_dataFolder);
            LoadAll();
        }

        // Tüm verileri yükle
        public void LoadAll()
        {
            Members = Load<List<Member>>("members.json") ?? new();
            Packages = Load<List<MembershipPackage>>("packages.json") ?? new();
            Subscriptions = Load<List<Subscription>>("subscriptions.json") ?? new();
            Payments = Load<List<Payment>>("payments.json") ?? new();
            Devices = Load<List<Device>>("devices.json") ?? new();
            MaintenanceRecords = Load<List<MaintenanceRecord>>("maintenance.json") ?? new();
            RepairRecords = Load<List<RepairRecord>>("repairs.json") ?? new();
            Users = Load<List<User>>("users.json") ?? new();
        }

        // Tüm verileri kaydet
        public void SaveAll()
        {
            Save("members.json", Members);
            Save("packages.json", Packages);
            Save("subscriptions.json", Subscriptions);
            Save("payments.json", Payments);
            Save("devices.json", Devices);
            Save("maintenance.json", MaintenanceRecords);
            Save("repairs.json", RepairRecords);
            Save("users.json", Users);
        }

        private T? Load<T>(string fileName)
        {
            var path = Path.Combine(_dataFolder, fileName);
            if (!File.Exists(path)) return default;

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        private void Save<T>(string fileName, T data)
        {
            var path = Path.Combine(_dataFolder, fileName);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}