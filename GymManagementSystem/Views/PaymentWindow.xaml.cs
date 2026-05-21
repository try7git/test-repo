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
    public partial class PaymentWindow : Window
    {
        public PaymentWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            MemberCombo.ItemsSource = App.MemberService.GetAllMembers();
            MemberCombo.SelectedIndex = 0;
            PaymentsGrid.ItemsSource = null;
            PaymentsGrid.ItemsSource = App.PaymentService.GetAllPayments();
        }

        private void MemberCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MemberCombo.SelectedItem is not Member member)
            {
                SubscriptionStatusBorder.Visibility = Visibility.Collapsed;
                return;
            }

            // Aktif abonelik var mı kontrol et (UC-05)
            var activeSubscription = App.SubscriptionService.GetActiveSubscription(member.Id);

            SubscriptionStatusBorder.Visibility = Visibility.Visible;

            if (activeSubscription != null)
            {
                SubscriptionStatusBorder.Background = new SolidColorBrush(Color.FromRgb(198, 246, 213));
                SubscriptionStatusText.Text = $"✅ Aktif Abonelik: {activeSubscription.MembershipPackage?.Name} — Bitis: {activeSubscription.EndDate:dd/MM/yyyy}";
                SubscriptionStatusText.Foreground = new SolidColorBrush(Color.FromRgb(39, 103, 73));

                // Paketten tutarı otomatik doldur
                if (activeSubscription.MembershipPackage != null)
                    AmountBox.Text = activeSubscription.MembershipPackage.Price.ToString();
            }
            else
            {
                SubscriptionStatusBorder.Background = new SolidColorBrush(Color.FromRgb(254, 215, 215));
                SubscriptionStatusText.Text = "⚠️ Aktif abonelik bulunamadi!";
                SubscriptionStatusText.Foreground = new SolidColorBrush(Color.FromRgb(155, 35, 53));
                AmountBox.Text = "";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemberCombo.SelectedItem is not Member member)
            {
                ShowMessage("❌ Lutfen uye secin!", false);
                return;
            }

            if (!decimal.TryParse(AmountBox.Text, out decimal amount))
            {
                ShowMessage("❌ Gecersiz tutar!", false);
                return;
            }

            // Aktif abonelik kontrolü (UC-05 etkinlik diyagramı)
            var activeSubscription = App.SubscriptionService.GetActiveSubscription(member.Id);
            if (activeSubscription == null)
            {
                ShowMessage("❌ Uye icin aktif veya bekleyen abonelik bulunamadi!", false);
                return;
            }

            var payment = new Payment
            {
                MemberId = member.Id,
                SubscriptionId = activeSubscription.Id,
                Amount = amount,
                PaymentMethod = (PaymentMethod)PaymentMethodCombo.SelectedIndex,
                Status = (PaymentStatus)PaymentStatusCombo.SelectedIndex,
                PaymentDate = DateTime.Now,
                Aciklama = $"{member.FullName} — {activeSubscription.MembershipPackage?.Name}"
            };

            var result = App.PaymentService.AddPayment(payment);

            if (!result)
            {
                ShowMessage("❌ Odeme eklenemedi! Tutar gecersiz.", false);
                return;
            }

            // Makbuz mesajı göster (UC-05 etkinlik diyagramı)
            ShowMessage($"✅ Odeme kaydedildi!\n📋 Makbuz: {member.FullName}\n💰 Tutar: {amount:N2} TL\n📅 Tarih: {DateTime.Now:dd/MM/yyyy HH:mm}", true);

            App.DataStore.Payments = App.PaymentService.GetAllPayments();
            App.DataStore.SaveAll();
            AmountBox.Text = "";
            LoadData();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
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