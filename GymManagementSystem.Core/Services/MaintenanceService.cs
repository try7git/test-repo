using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;

namespace GymManagementSystem.Core.Services
{
    public class MaintenanceService
    {
        private readonly List<Device> _devices = new();
        private readonly List<MaintenanceRecord> _maintenanceRecords = new();
        private readonly List<RepairRecord> _repairRecords = new();

        // Cihaz işlemleri
        public List<Device> GetAllDevices()
        {
            return _devices;
        }

        public Device? GetDeviceById(int id)
        {
            return _devices.FirstOrDefault(d => d.Id == id);
        }

        public void AddDevice(Device device)
        {
            device.Id = _devices.Count > 0 ? _devices.Max(d => d.Id) + 1 : 1;
            _devices.Add(device);
        }

        public bool UpdateDeviceStatus(int deviceId, DeviceStatus status)
        {
            var device = GetDeviceById(deviceId);
            if (device == null) return false;

            device.Status = status;
            return true;
        }

        // Bakım işlemleri (spor salonunda yapılır)
        public bool AddMaintenanceRecord(MaintenanceRecord record)
        {
            var device = GetDeviceById(record.DeviceId);
            if (device == null) return false;

            // Maliyet negatif olamaz
            if (record.Cost < 0) return false;

            record.Id = _maintenanceRecords.Count > 0 ?
                _maintenanceRecords.Max(m => m.Id) + 1 : 1;
            record.Location = "Spor Salonu";
            _maintenanceRecords.Add(record);

            // Bakım başlatılınca cihaz durumu "Bakımda" olur (UC-07)
            device.Status = DeviceStatus.UnderMaintenance;

            return true;
        }

        public bool CompleteMaintenanceRecord(int recordId)
        {
            var record = _maintenanceRecords.FirstOrDefault(m => m.Id == recordId);
            if (record == null) return false;

            record.Status = MaintenanceStatus.Completed;

            // Bakım tamamlanınca cihaz "Aktif" duruma döner (UC-07)
            UpdateDeviceStatus(record.DeviceId, DeviceStatus.Active);
            return true;
        }

        public List<MaintenanceRecord> GetMaintenanceByDevice(int deviceId)
        {
            return _maintenanceRecords
                .Where(m => m.DeviceId == deviceId).ToList();
        }

        // Tamir işlemleri (teknik serviste yapılır)
        public bool AddRepairRecord(RepairRecord record)
        {
            var device = GetDeviceById(record.DeviceId);
            if (device == null) return false;

            // Teknik servis bilgisi zorunlu (UC-07)
            if (string.IsNullOrWhiteSpace(record.ServiceCompany)) return false;

            // Maliyet negatif olamaz (UC-07)
            if (record.Cost < 0) return false;

            record.Id = _repairRecords.Count > 0 ?
                _repairRecords.Max(r => r.Id) + 1 : 1;
            record.Location = "Teknik Servis";
            _repairRecords.Add(record);

            // Tamir başlatılınca cihaz durumu "Tamirde" olur (UC-07)
            device.Status = DeviceStatus.UnderRepair;

            return true;
        }

        public bool CompleteRepairRecord(int recordId)
        {
            var record = _repairRecords.FirstOrDefault(r => r.Id == recordId);
            if (record == null) return false;

            record.Status = RepairStatus.Completed;

            // Tamir tamamlanınca cihaz "Aktif" duruma döner (UC-07)
            UpdateDeviceStatus(record.DeviceId, DeviceStatus.Active);
            return true;
        }

        public List<RepairRecord> GetRepairsByDevice(int deviceId)
        {
            return _repairRecords
                .Where(r => r.DeviceId == deviceId).ToList();
        }

        // Maliyet sorguları
        public decimal GetTotalMaintenanceCostByMonth(int month, int year)
        {
            var maintenanceCost = _maintenanceRecords
                .Where(m => m.MaintenanceDate.Month == month &&
                            m.MaintenanceDate.Year == year)
                .Sum(m => m.Cost);

            var repairCost = _repairRecords
                .Where(r => r.RepairDate.Month == month &&
                            r.RepairDate.Year == year)
                .Sum(r => r.Cost);

            return maintenanceCost + repairCost;
        }

        public decimal GetTotalCostByDevice(int deviceId)
        {
            var device = GetDeviceById(deviceId);
            if (device == null) return 0;
            return device.TotalMaintenanceCost;
        }
    }
}