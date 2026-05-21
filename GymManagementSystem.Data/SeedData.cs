using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;

namespace GymManagementSystem.Data
{
    public static class SeedData
    {
        public static void Initialize(JsonDataStore store)
        {
            // Zaten veri varsa tekrar ekleme
            if (store.Members.Any()) return;

            // Örnek Üyeler
            store.Members.AddRange(new List<Member>
            {
                new Member
                {
                    Id = 1,
                    FirstName = "Ahmet",
                    LastName = "Yılmaz",
                    NationalId = "12345678901",
                    BirthDate = new DateTime(1990, 5, 15),
                    Phone = "05321234567",
                    Email = "ahmet@email.com",
                    RegistrationDate = DateTime.Today,
                    IsActive = true,
                    MedicalReport = new MedicalReport
                    {
                        Id = 1,
                        MemberId = 1,
                        ReportDate = DateTime.Today.AddMonths(-6), // Geçerli
                        IssuedBy = "Dr. Mehmet Kaya"
                    }
                },
                new Member
                {
                    Id = 2,
                    FirstName = "Ayşe",
                    LastName = "Demir",
                    NationalId = "98765432101",
                    BirthDate = new DateTime(1995, 8, 20),
                    Phone = "05419876543",
                    Email = "ayse@email.com",
                    RegistrationDate = DateTime.Today,
                    IsActive = true,
                    MedicalReport = new MedicalReport
                    {
                        Id = 2,
                        MemberId = 2,
                        ReportDate = DateTime.Today.AddYears(-1).AddDays(15), // Yakında dolacak
                        IssuedBy = "Dr. Ali Çelik"
                    }
                },
                new Member
                {
                    Id = 3,
                    FirstName = "Mehmet",
                    LastName = "Kara",
                    NationalId = "11223344556",
                    BirthDate = new DateTime(1988, 3, 10),
                    Phone = "05551122334",
                    Email = "mehmet@email.com",
                    RegistrationDate = DateTime.Today,
                    IsActive = true,
                    MedicalReport = new MedicalReport
                    {
                        Id = 3,
                        MemberId = 3,
                        ReportDate = DateTime.Today.AddYears(-2), // Geçersiz
                        IssuedBy = "Dr. Zeynep Arslan"
                    }
                }
            });

            // Örnek Paketler
            store.Packages.AddRange(new List<MembershipPackage>
            {
                new MembershipPackage
                {
                    Id = 1,
                    Name = "Haftada 2 Gün",
                    PackageType = PackageType.TwoDaysPerWeek,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(22, 0, 0),
                    DurationMonths = 1,
                    Price = 500,
                    IsActive = true
                },
                new MembershipPackage
                {
                    Id = 2,
                    Name = "Haftada 3 Gün",
                    PackageType = PackageType.ThreeDaysPerWeek,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(22, 0, 0),
                    DurationMonths = 1,
                    Price = 700,
                    IsActive = true
                },
                new MembershipPackage
                {
                    Id = 3,
                    Name = "Her Gün",
                    PackageType = PackageType.EveryDay,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(22, 0, 0),
                    DurationMonths = 1,
                    Price = 1000,
                    IsActive = true
                }
            });

            // Örnek Cihazlar
            store.Devices.AddRange(new List<Device>
            {
                new Device
                {
                    Id = 1,
                    Name = "Koşu Bandı",
                    Brand = "Technogym",
                    Model = "Run 500",
                    PurchaseDate = new DateTime(2022, 1, 10),
                    Status = DeviceStatus.Active
                },
                new Device
                {
                    Id = 2,
                    Name = "Kürek Makinesi",
                    Brand = "Concept2",
                    Model = "RowErg",
                    PurchaseDate = new DateTime(2021, 6, 15),
                    Status = DeviceStatus.Active
                },
                new Device
                {
                    Id = 3,
                    Name = "Bisiklet",
                    Brand = "Life Fitness",
                    Model = "IC5",
                    PurchaseDate = new DateTime(2023, 3, 20),
                    Status = DeviceStatus.UnderRepair
                }
            });

            // Örnek Kullanıcılar
            store.Users.AddRange(new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    FullName = "Sistem Yöneticisi",
                    Role = UserRole.Manager,
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Username = "resepsiyon",
                    Password = "res123",
                    FullName = "Resepsiyonist",
                    Role = UserRole.Receptionist,
                    IsActive = true
                },
                new User
                {
                    Id = 3,
                    Username = "bakim",
                    Password = "bakim123",
                    FullName = "Bakım Görevlisi",
                    Role = UserRole.MaintenanceStaff,
                    IsActive = true
                },
                new User
                {
                     Id = 4,
                     Username = "uye",
                     Password = "uye123",
                     FullName = "Test Uyesi",
                     Role = UserRole.Member,
                     IsActive = true
                }

            });

            store.SaveAll();
        }
    }
}