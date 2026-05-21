using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using GymManagementSystem.Core.Entities;

namespace GymManagementSystem.Views
{
    public partial class PackageWindow : Window
    {
        private int? _selectedPackageId = null;

        public PackageWindow()
        {
            InitializeComponent();
            LoadPackages();
        }

        private void LoadPackages()
        {
            PackagesGrid.ItemsSource = null;
            PackagesGrid.ItemsSource = App.PackageService.GetAllPackages();
        }

        private void PackagesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PackagesGrid.SelectedItem is MembershipPackage package)
            {
                _selectedPackageId = package.Id;
                NameBox.Text = package.Name;
                PackageTypeCombo.SelectedIndex = (int)package.PackageType;
                StartTimeBox.Text = package.StartTime.ToString(@"hh\:mm");
                EndTimeBox.Text = package.EndTime.ToString(@"hh\:mm");
                DurationBox.Text = package.DurationMonths.ToString();
                PriceBox.Text = package.Price.ToString();
                IsActiveCheck.IsChecked = package.IsActive;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(PriceBox.Text))
            {
                ShowMessage("❌ Paket adi ve ucret zorunludur!", false);
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, out decimal price) ||
                !int.TryParse(DurationBox.Text, out int duration))
            {
                ShowMessage("❌ Gecersiz ucret veya sure!", false);
                return;
            }

            if (!TimeSpan.TryParse(StartTimeBox.Text, out TimeSpan startTime) ||
                !TimeSpan.TryParse(EndTimeBox.Text, out TimeSpan endTime))
            {
                ShowMessage("❌ Gecersiz saat araligi! (HH:mm formatinda giriniz)", false);
                return;
            }

            var package = new MembershipPackage
            {
                Name = NameBox.Text,
                PackageType = (PackageType)PackageTypeCombo.SelectedIndex,
                StartTime = startTime,
                EndTime = endTime,
                DurationMonths = duration,
                Price = price,
                IsActive = IsActiveCheck.IsChecked ?? true
            };

            if (_selectedPackageId.HasValue)
            {
                package.Id = _selectedPackageId.Value;
                App.PackageService.UpdatePackage(package);
                ShowMessage("✅ Paket basariyla guncellendi!", true);
            }
            else
            {
                // Yeni AddPackage artık (bool, string) döndürüyor
                var (success, message) = App.PackageService.AddPackage(package);
                if (!success)
                {
                    ShowMessage($"❌ {message}", false);
                    return;
                }
                ShowMessage($"✅ {message}", true);
            }

            App.DataStore.Packages = App.PackageService.GetAllPackages();
            App.DataStore.SaveAll();
            LoadPackages();
            ClearForm();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPackageId == null)
            {
                ShowMessage("❌ Lutfen bir paket secin!", false);
                return;
            }

            var result = MessageBox.Show("Bu paketi silmek istediginize emin misiniz?",
                "Silme Onay", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                App.PackageService.DeletePackage(_selectedPackageId.Value);
                App.DataStore.Packages = App.PackageService.GetAllPackages();
                App.DataStore.SaveAll();
                LoadPackages();
                ClearForm();
                ShowMessage("✅ Paket silindi!", true);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadPackages();
            ClearForm();
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            MessageBorder.Visibility = Visibility.Visible;
            MessageText.Text = message;
            MessageBorder.Background = isSuccess
                ? new SolidColorBrush(Color.FromRgb(198, 246, 213))
                : new SolidColorBrush(Color.FromRgb(254, 215, 215));
            MessageText.Foreground = isSuccess
                ? new SolidColorBrush(Color.FromRgb(39, 103, 73))
                : new SolidColorBrush(Color.FromRgb(155, 35, 53));
        }

        private void ClearForm()
        {
            _selectedPackageId = null;
            NameBox.Text = "";
            PackageTypeCombo.SelectedIndex = 0;
            StartTimeBox.Text = "08:00";
            EndTimeBox.Text = "22:00";
            DurationBox.Text = "1";
            PriceBox.Text = "";
            IsActiveCheck.IsChecked = true;
            MessageBorder.Visibility = Visibility.Collapsed;
        }
    }
}