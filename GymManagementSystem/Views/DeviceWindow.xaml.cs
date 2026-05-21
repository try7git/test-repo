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
    public class MaintenanceItem
    {
        public string Type { get; set; } = "";
        public DateTime Date { get; set; }
        public string Description { get; set; } = "";
        public decimal Cost { get; set; }
        public string Location { get; set; } = "";
    }

    public partial class DeviceWindow : Window
    {
        public DeviceWindow()
        {
            InitializeComponent();
            LoadDevices();
            RecordDatePicker.SelectedDate = DateTime.Today;
        }

        private void LoadDevices()
        {
            DevicesGrid.ItemsSource = null;
            DevicesGrid.ItemsSource = App.MaintenanceService.GetAllDevices();
            DeviceCombo.ItemsSource = App.MaintenanceService.GetAllDevices();
        }

        private void DevicesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DevicesGrid.SelectedItem is Device device)
            {
                DeviceCombo.SelectedItem = device;
                LoadMaintenanceRecords(device.Id);
            }
        }

        private void DeviceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeviceCombo.SelectedItem is Device device)
                LoadMaintenanceRecords(device.Id);
        }

        private void LoadMaintenanceRecords(int deviceId)
        {
            var items = new List<MaintenanceItem>();

            foreach (var m in App.MaintenanceService.GetMaintenanceByDevice(deviceId))
                items.Add(new MaintenanceItem
                {
                    Type = "Bakim",
                    Date = m.MaintenanceDate,
                    Description = m.Description,
                    Cost = m.Cost,
                    Location = m.Location
                });

            foreach (var r in App.MaintenanceService.GetRepairsByDevice(deviceId))
                items.Add(new MaintenanceItem
                {
                    Type = "Tamir",
                    Date = r.RepairDate,
                    Description = r.Description,
                    Cost = r.Cost,
                    Location = r.Location
                });

            MaintenanceGrid.ItemsSource = items.OrderByDescending(i => i.Date).ToList();
        }

        private void RecordTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StaffLabel == null) return;
            StaffLabel.Text = RecordTypeCombo.SelectedIndex == 1
                ? "Teknik Servis" : "Gorevli";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceCombo.SelectedItem is not Device device)
            {
                ShowMessage("❌ Lutfen cihaz secin!", false);
                return;
            }

            if (!decimal.TryParse(CostBox.Text, out decimal cost))
            {
                ShowMessage("❌ Gecersiz maliyet!", false);
                return;
            }

            bool result;

            if (RecordTypeCombo.SelectedIndex == 0)
            {
                result = App.MaintenanceService.AddMaintenanceRecord(new MaintenanceRecord
                {
                    DeviceId = device.Id,
                    MaintenanceDate = RecordDatePicker.SelectedDate ?? DateTime.Today,
                    Description = DescriptionBox.Text,
                    AssignedStaff = StaffBox.Text,
                    Cost = cost
                });
            }
            else
            {
                result = App.MaintenanceService.AddRepairRecord(new RepairRecord
                {
                    DeviceId = device.Id,
                    RepairDate = RecordDatePicker.SelectedDate ?? DateTime.Today,
                    Description = DescriptionBox.Text,
                    ServiceCompany = StaffBox.Text,
                    Cost = cost
                });
            }

            if (!result)
            {
                ShowMessage("❌ Kayit eklenemedi! Teknik servis bilgisi eksik olabilir.", false);
                return;
            }

            ShowMessage("✅ Kayit basariyla eklendi!", true);
            App.DataStore.SaveAll();
            LoadMaintenanceRecords(device.Id);
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
            DescriptionBox.Text = "";
            StaffBox.Text = "";
            CostBox.Text = "";
            RecordDatePicker.SelectedDate = DateTime.Today;
            MessageBorder.Visibility = Visibility.Collapsed;
        }
    }
}
