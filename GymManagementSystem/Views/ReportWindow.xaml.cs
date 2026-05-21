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



namespace GymManagementSystem.Views
{
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();
            MonthCombo.SelectedIndex = DateTime.Now.Month - 1;
            LoadNotifications();
        }

        private void MonthlyReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(YearBox.Text, out int year))
            {
                MessageBox.Show("Gecersiz yil!", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var month = MonthCombo.SelectedIndex + 1;
            var report = App.ReportService.GetMonthlyReport(month, year);

            ReportGrid.ItemsSource = new[] { report };
            UpdateSummary(report.TotalIncome, report.TotalExpense, report.NetBalance);
        }

        private void YearlyReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(YearBox.Text, out int year))
            {
                MessageBox.Show("Gecersiz yil!", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var reports = App.ReportService.GetYearlyReport(year);
            ReportGrid.ItemsSource = reports;

            UpdateSummary(
                App.ReportService.GetYearlyTotalIncome(year),
                App.ReportService.GetYearlyTotalExpense(year),
                App.ReportService.GetYearlyNetBalance(year));
        }

        private void UpdateSummary(decimal income, decimal expense, decimal net)
        {
            TotalIncomeText.Text = $"{income:N2} TL";
            TotalExpenseText.Text = $"{expense:N2} TL";
            NetBalanceText.Text = $"{net:N2} TL";
            NetBalanceText.Foreground = net >= 0
                ? System.Windows.Media.Brushes.White
                : new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(255, 200, 200));
        }

        private void LoadNotifications()
        {
            var notifications = App.NotificationService.GetAllNotifications();
            NotificationGrid.ItemsSource = notifications;
            NotificationCountText.Text = notifications.Count.ToString();
        }
    }
}