using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GymManagementSystem.Core.Entities;



namespace GymManagementSystem.Core.Services
{
    public class PackageService
    {
        private readonly List<MembershipPackage> _packages = new();

        public List<MembershipPackage> GetAllPackages()
        {
            return _packages;
        }

        public List<MembershipPackage> GetActivePackages()
        {
            return _packages.Where(p => p.IsActive).ToList();
        }

        public MembershipPackage? GetById(int id)
        {
            return _packages.FirstOrDefault(p => p.Id == id);
        }

        public (bool Success, string Message) AddPackage(MembershipPackage package)
        {
            // Ücret 0 veya negatif olamaz (UC-02)
            if (package.Price <= 0)
                return (false, "Paket ucreti 0 veya negatif olamaz!");

            // Saat aralığı geçerli olmalı
            if (package.StartTime >= package.EndTime)
                return (false, "Baslangic saati bitis saatinden buyuk olamaz!");

            // Aynı isimde aktif paket olamaz (UC-02)
            if (_packages.Any(p => p.IsActive &&
                p.Name.ToLower() == package.Name.ToLower()))
                return (false, "Ayni isimde aktif bir paket zaten mevcut!");

            // Paket adı boş olamaz
            if (string.IsNullOrWhiteSpace(package.Name))
                return (false, "Paket adi bos olamaz!");

            // Süre geçerli olmalı
            if (package.DurationMonths <= 0)
                return (false, "Gecerlilik suresi 0'dan buyuk olmalidir!");

            package.Id = _packages.Count > 0 ? _packages.Max(p => p.Id) + 1 : 1;
            _packages.Add(package);
            return (true, "Paket basariyla eklendi!");
        }

        public bool UpdatePackage(MembershipPackage updated)
        {
            var existing = GetById(updated.Id);
            if (existing == null) return false;

            existing.Name = updated.Name;
            existing.PackageType = updated.PackageType;
            existing.StartTime = updated.StartTime;
            existing.EndTime = updated.EndTime;
            existing.DurationMonths = updated.DurationMonths;
            existing.Price = updated.Price;
            existing.IsActive = updated.IsActive;
            return true;
        }

        public bool SetPackageStatus(int id, bool isActive)
        {
            var package = GetById(id);
            if (package == null) return false;

            package.IsActive = isActive;
            return true;
        }

        public bool DeletePackage(int id)
        {
            var package = GetById(id);
            if (package == null) return false;

            _packages.Remove(package);
            return true;
        }
    }
}