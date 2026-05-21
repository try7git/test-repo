using System.Configuration;
using System.Data;
using GymManagementSystem.Core.Services;
using GymManagementSystem.Data;
using System.Windows;

namespace GymManagementSystem
{
    public partial class App : Application
    {
        // Tüm uygulama boyunca erişilebilir servisler
        public static SubscriptionService SubscriptionService { get; private set; } = new();
        public static JsonDataStore DataStore { get; private set; } = new();
        public static AuthService AuthService { get; private set; } = new();
        public static MemberService MemberService { get; private set; } = new();
        public static PackageService PackageService { get; private set; } = new();
        public static PaymentService PaymentService { get; private set; } = new();
        public static MaintenanceService MaintenanceService { get; private set; } = new();
        public static ReportService ReportService { get; private set; } = new(
            new PaymentService(), new MaintenanceService());
        public static NotificationService NotificationService { get; private set; } = new(
            new MemberService());

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Veriyi yükle
            DataStore = new JsonDataStore();

            // Örnek verileri yükle (ilk çalıştırmada)
            SeedData.Initialize(DataStore);

            // Servislere veriyi aktar
            // YENİ — validasyon bypass ederek yükle
            foreach (var member in DataStore.Members)
                MemberService.LoadMember(member);

            foreach (var package in DataStore.Packages)
                PackageService.AddPackage(package);

            foreach (var payment in DataStore.Payments)
                PaymentService.AddPayment(payment);

            foreach (var device in DataStore.Devices)
                MaintenanceService.AddDevice(device);

            foreach (var user in DataStore.Users)
                AuthService.AddUser(user);

            // Giriş ekranını aç
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
        }

        // Uygulama kapanırken kaydet
        protected override void OnExit(ExitEventArgs e)
        {
            DataStore.Members = MemberService.GetAllMembers();
            DataStore.Packages = PackageService.GetAllPackages();
            DataStore.Payments = PaymentService.GetAllPayments();
            DataStore.Subscriptions = SubscriptionService.GetAllSubscriptions();
            DataStore.SaveAll();

            base.OnExit(e);
        }
    }
}
