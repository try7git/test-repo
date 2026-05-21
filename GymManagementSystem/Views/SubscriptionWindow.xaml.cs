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
    public partial class SubscriptionWindow : Window
    {
        private Subscription? _selectedSubscription = null;

        public SubscriptionWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MemberCombo.ItemsSource = App.MemberService.GetAllMembers();
            PackagesListBox.ItemsSource = App.PackageService.GetActivePackages();
            SubscriptionsGrid.ItemsSource = null;
            SubscriptionsGrid.ItemsSource = App.SubscriptionService.GetAllSubscriptions();
        }

        private void SubscriptionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSubscription = SubscriptionsGrid.SelectedItem as Subscription;
        }

        private void MemberCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MemberCombo.SelectedItem is not Member member)
            {
                MemberStatusBorder.Visibility = Visibility.Collapsed;
                return;
            }

            MemberStatusBorder.Visibility = Visibility.Visible;

            if (member.MedicalReport == null)
            {
                MemberStatusBorder.Background = new SolidColorBrush(Color.FromRgb(254, 215, 215));
                MemberStatusText.Text = "❌ Saglik raporu yok!";
                MemberStatusText.Foreground = new SolidColorBrush(Color.FromRgb(155, 35, 53));
            }
            else
            {
                switch (member.MedicalReport.Status)
                {
                    case ReportStatus.Valid:
                        MemberStatusBorder.Background = new SolidColorBrush(Color.FromRgb(198, 246, 213));
                        MemberStatusText.Text = $"✅ Rapor Gecerli — Bitis: {member.MedicalReport.ExpiryDate:dd/MM/yyyy}";
                        MemberStatusText.Foreground = new SolidColorBrush(Color.FromRgb(39, 103, 73));
                        break;
                    case ReportStatus.ExpiringSoon:
                        MemberStatusBorder.Background = new SolidColorBrush(Color.FromRgb(254, 252, 191));
                        MemberStatusText.Text = $"⚠️ Rapor Yakin Doluyor — Bitis: {member.MedicalReport.ExpiryDate:dd/MM/yyyy}";
                        MemberStatusText.Foreground = new SolidColorBrush(Color.FromRgb(116, 66, 16));
                        break;
                    case ReportStatus.Expired:
                        MemberStatusBorder.Background = new SolidColorBrush(Color.FromRgb(254, 215, 215));
                        MemberStatusText.Text = "❌ Rapor Suresi Dolmus! Abonelik baslatılamaz.";
                        MemberStatusText.Foreground = new SolidColorBrush(Color.FromRgb(155, 35, 53));
                        break;
                }
            }

            UpdateSummary();
        }

        private void PackagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            if (MemberCombo.SelectedItem is Member member &&
                PackagesListBox.SelectedItem is MembershipPackage package)
            {
                SummaryBorder.Visibility = Visibility.Visible;
                SummaryMember.Text = $"👤 Uye: {member.FullName}";
                SummaryPackage.Text = $"📦 Paket: {package.Name}";
                SummaryPrice.Text = $"💰 Ucret: {package.Price:N2} TL";
                SummaryDates.Text = $"📅 {DateTime.Today:dd/MM/yyyy} → {DateTime.Today.AddMonths(package.DurationMonths):dd/MM/yyyy}";
            }
            else
            {
                SummaryBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemberCombo.SelectedItem is not Member member)
            {
                ShowMessage("❌ Lutfen uye secin!", false);
                return;
            }

            if (PackagesListBox.SelectedItem is not MembershipPackage package)
            {
                ShowMessage("❌ Lutfen paket secin!", false);
                return;
            }

            var (success, message) = App.SubscriptionService.CreateSubscription(
                member, package, (PaymentMethod)PaymentMethodCombo.SelectedIndex);

            if (!success)
            {
                ShowMessage($"❌ {message}", false);
                return;
            }

            // Ödeme kaydı oluştur
            App.PaymentService.AddPayment(new Payment
            {
                MemberId = member.Id,
                SubscriptionId = App.SubscriptionService
                    .GetAllSubscriptions().Last().Id,
                Amount = package.Price,
                PaymentMethod = (PaymentMethod)PaymentMethodCombo.SelectedIndex,
                Status = PaymentStatus.Completed,
                PaymentDate = DateTime.Now
            });

            App.DataStore.Subscriptions = App.SubscriptionService.GetAllSubscriptions();
            App.DataStore.Payments = App.PaymentService.GetAllPayments();
            App.DataStore.SaveAll();

            ShowMessage($"✅ {message}", true);
            LoadData();
        }

        private void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSubscription == null)
            {
                ShowMessage("❌ Lutfen listeden bir abonelik secin!", false);
                return;
            }

            var (success, message) = App.SubscriptionService.RenewSubscription(
                _selectedSubscription.Id,
                (PaymentMethod)PaymentMethodCombo.SelectedIndex);

            if (!success)
            {
                ShowMessage($"❌ {message}", false);
                return;
            }

            // Yenileme ödemesi oluştur
            App.PaymentService.AddPayment(new Payment
            {
                MemberId = _selectedSubscription.MemberId,
                SubscriptionId = _selectedSubscription.Id,
                Amount = _selectedSubscription.MembershipPackage?.Price ?? 0,
                PaymentMethod = (PaymentMethod)PaymentMethodCombo.SelectedIndex,
                Status = PaymentStatus.Completed,
                PaymentDate = DateTime.Now
            });

            App.DataStore.Subscriptions = App.SubscriptionService.GetAllSubscriptions();
            App.DataStore.Payments = App.PaymentService.GetAllPayments();
            App.DataStore.SaveAll();

            ShowMessage($"✅ {message}", true);
            LoadData();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSubscription == null)
            {
                ShowMessage("❌ Lutfen listeden bir abonelik secin!", false);
                return;
            }

            var result = MessageBox.Show(
                "Bu aboneligi iptal etmek istediginize emin misiniz?",
                "Iptal Onay",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                App.SubscriptionService.CancelSubscription(_selectedSubscription.Id);
                App.DataStore.Subscriptions = App.SubscriptionService.GetAllSubscriptions();
                App.DataStore.SaveAll();
                ShowMessage("✅ Abonelik iptal edildi!", true);
                LoadData();
            }
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
    }
}