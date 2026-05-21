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
    public class MaintenanceServiceTests
    {
        private MaintenanceService _maintenanceService = new();

        [TestInitialize]
        public void Setup()
        {
            _maintenanceService = new MaintenanceService();

            // Test için varsayılan cihaz ekle
            _maintenanceService.AddDevice(new Device
            {
                Id = 1,
                Name = "Koşu Bandı",
                Brand = "TestMarka",
                Model = "TB-100",
                PurchaseDate = DateTime.Today.AddYears(-1)
            });
        }

        [TestMethod]
        public void AddMaintenanceRecord_ValidRecord_ReturnsTrue()
        {
            // Arrange
            var record = new MaintenanceRecord
            {
                DeviceId = 1,
                MaintenanceDate = DateTime.Today,
                Description = "Periyodik bakım",
                AssignedStaff = "Ahmet Usta",
                Cost = 200
            };

            // Act
            var result = _maintenanceService.AddMaintenanceRecord(record);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AddMaintenanceRecord_InvalidDevice_ReturnsFalse()
        {
            // Arrange
            var record = new MaintenanceRecord
            {
                DeviceId = 999, // Olmayan cihaz
                MaintenanceDate = DateTime.Today,
                Description = "Bakım",
                Cost = 200
            };

            // Act
            var result = _maintenanceService.AddMaintenanceRecord(record);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddRepairRecord_WithoutServiceCompany_ReturnsFalse()
        {
            // Arrange
            var record = new RepairRecord
            {
                DeviceId = 1,
                RepairDate = DateTime.Today,
                Description = "Tamir",
                ServiceCompany = "", // Teknik servis boş olamaz
                Cost = 500
            };

            // Act
            var result = _maintenanceService.AddRepairRecord(record);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddRepairRecord_NegativeCost_ReturnsFalse()
        {
            // Arrange
            var record = new RepairRecord
            {
                DeviceId = 1,
                RepairDate = DateTime.Today,
                Description = "Tamir",
                ServiceCompany = "ABC Teknik Servis",
                Cost = -100 // Negatif maliyet olamaz
            };

            // Act
            var result = _maintenanceService.AddRepairRecord(record);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetTotalMaintenanceCostByMonth_ReturnsCorrectAmount()
        {
            // Arrange
            _maintenanceService.AddMaintenanceRecord(new MaintenanceRecord
            {
                DeviceId = 1,
                MaintenanceDate = DateTime.Today,
                Description = "Bakım",
                AssignedStaff = "Ahmet Usta",
                Cost = 300
            });

            _maintenanceService.AddRepairRecord(new RepairRecord
            {
                DeviceId = 1,
                RepairDate = DateTime.Today,
                Description = "Tamir",
                ServiceCompany = "ABC Servis",
                Cost = 500
            });

            // Act
            var total = _maintenanceService.GetTotalMaintenanceCostByMonth(
                DateTime.Today.Month, DateTime.Today.Year);

            // Assert
            Assert.AreEqual(800, total);
        }
    }
}