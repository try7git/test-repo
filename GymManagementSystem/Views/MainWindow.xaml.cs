using GymManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GymManagementSystem.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WelcomeText.Text = $"Hosgeldiniz, {App.AuthService.CurrentUser?.FullName}";
            RolText.Text = $"Rol: {GetRoleName()}";
            LoadNotifications();
            ApplyRoleBasedMenu();
        }

        private string GetRoleName()
        {
            return App.AuthService.CurrentUser?.Role switch
            {
                UserRole.Manager => "Yonetici",
                UserRole.Receptionist => "Resepsiyonist",
                UserRole.MaintenanceStaff => "Bakim Gorevlisi",
                UserRole.Trainer => "Antrenor",
                UserRole.Member => "Uye",
                _ => "Bilinmiyor"
            };
        }

        private void ApplyRoleBasedMenu()
        {
            var role = App.AuthService.CurrentUser?.Role;

            switch (role)
            {
                case UserRole.Manager:
                    // Yönetici her şeyi görür, değişiklik yok
                    break;

                case UserRole.Receptionist:
                    // Üye, Abonelik, Ödeme — Paket, Cihaz, Rapor gizli
                    PackageButton.Visibility = Visibility.Collapsed;
                    DeviceButton.Visibility = Visibility.Collapsed;
                    ReportButton.Visibility = Visibility.Collapsed;
                    break;

                case UserRole.MaintenanceStaff:
                    // Sadece Cihaz/Bakım
                    MemberButton.Visibility = Visibility.Collapsed;
                    PackageButton.Visibility = Visibility.Collapsed;
                    SubscriptionButton.Visibility = Visibility.Collapsed;
                    PaymentButton.Visibility = Visibility.Collapsed;
                    ReportButton.Visibility = Visibility.Collapsed;
                    break;

                case UserRole.Trainer:
                    // Sadece Üye listesi
                    PackageButton.Visibility = Visibility.Collapsed;
                    SubscriptionButton.Visibility = Visibility.Collapsed;
                    PaymentButton.Visibility = Visibility.Collapsed;
                    DeviceButton.Visibility = Visibility.Collapsed;
                    ReportButton.Visibility = Visibility.Collapsed;
                    break;

                case UserRole.Member:
                    // Sadece Abonelik ve Ödeme
                    MemberButton.Visibility = Visibility.Collapsed;
                    PackageButton.Visibility = Visibility.Collapsed;
                    DeviceButton.Visibility = Visibility.Collapsed;
                    ReportButton.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void LoadNotifications()
        {
            var notifications = App.NotificationService.GetAllNotifications();
            if (notifications.Count > 0)
            {
                NotificationBanner.Visibility = Visibility.Visible;
                NotificationText.Text = $"⚠️  {notifications.Count} adet saglik raporu uyarisi var!";
            }
        }

        private void MemberButton_Click(object sender, RoutedEventArgs e)
        {
            new MemberWindow().ShowDialog();
        }

        private void PackageButton_Click(object sender, RoutedEventArgs e)
        {
            new PackageWindow().ShowDialog();
        }

        private void SubscriptionButton_Click(object sender, RoutedEventArgs e)
        {
            new SubscriptionWindow().ShowDialog();
        }

        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            new PaymentWindow().ShowDialog();
        }

        private void DeviceButton_Click(object sender, RoutedEventArgs e)
        {
            new DeviceWindow().ShowDialog();
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            new ReportWindow().ShowDialog();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            App.AuthService.Logout();
            new LoginWindow().Show();
            this.Close();
        }
    }
}