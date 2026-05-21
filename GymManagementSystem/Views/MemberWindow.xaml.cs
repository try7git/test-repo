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
    public partial class MemberWindow : Window
    {
        private int? _selectedMemberId = null;

        public MemberWindow()
        {
            InitializeComponent();
            LoadMembers();
        }

        private void LoadMembers()
        {
            MembersGrid.ItemsSource = null;
            MembersGrid.ItemsSource = App.MemberService.GetAllMembers();
        }

        private void MembersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MembersGrid.SelectedItem is Member member)
            {
                _selectedMemberId = member.Id;
                FirstNameBox.Text = member.FirstName;
                LastNameBox.Text = member.LastName;
                NationalIdBox.Text = member.NationalId;
                BirthDatePicker.SelectedDate = member.BirthDate;
                PhoneBox.Text = member.Phone;
                EmailBox.Text = member.Email;

                if (member.MedicalReport != null)
                {
                    ReportDatePicker.SelectedDate = member.MedicalReport.ReportDate;
                    IssuedByBox.Text = member.MedicalReport.IssuedBy;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(NationalIdBox.Text))
            {
                ShowMessage("❌ Ad ve TC Kimlik No zorunludur!", false);
                return;
            }

            // Rapor tarihi kontrolü
            if (ReportDatePicker.SelectedDate.HasValue &&
                ReportDatePicker.SelectedDate.Value < DateTime.Today.AddYears(-1))
            {
                ShowMessage("❌ Saglik raporu 1 yildan eski! Guncel rapor gerekli.", false);
                return;
            }

            var member = new Member
            {
                FirstName = FirstNameBox.Text,
                LastName = LastNameBox.Text,
                NationalId = NationalIdBox.Text,
                BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Today,
                Phone = PhoneBox.Text,
                Email = EmailBox.Text,
                MedicalReport = ReportDatePicker.SelectedDate.HasValue
                    ? new MedicalReport
                    {
                        ReportDate = ReportDatePicker.SelectedDate.Value,
                        IssuedBy = IssuedByBox.Text
                    }
                    : null
            };

            if (_selectedMemberId.HasValue)
            {
                member.Id = _selectedMemberId.Value;
                var updateResult = App.MemberService.UpdateMember(member);
                if (!updateResult)
                {
                    ShowMessage("❌ Uye guncellenemedi!", false);
                    return;
                }
                ShowMessage("✅ Uye basariyla guncellendi!", true);
            }
            else
            {
                var result = App.MemberService.AddMember(member);
                if (!result)
                {
                    ShowMessage("❌ Bu TC ile kayit var veya rapor suresi dolmus!", false);
                    return;
                }
                ShowMessage("✅ Uye basariyla eklendi!", true);
            }

            App.DataStore.Members = App.MemberService.GetAllMembers();
            App.DataStore.SaveAll();

            // Grid'i zorla yenile
            var members = App.MemberService.GetAllMembers();
            MembersGrid.ItemsSource = null;
            MembersGrid.ItemsSource = members;

            ClearForm();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedMemberId == null)
            {
                ShowMessage("❌ Lutfen bir uye secin!", false);
                return;
            }

            var result = MessageBox.Show("Bu uyeyi silmek istediginize emin misiniz?",
                "Silme Onay", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                App.MemberService.DeleteMember(_selectedMemberId.Value);
                App.DataStore.Members = App.MemberService.GetAllMembers();
                App.DataStore.SaveAll();
                LoadMembers();
                ClearForm();
                ShowMessage("✅ Uye silindi!", true);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMembers();
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
            _selectedMemberId = null;
            FirstNameBox.Text = "";
            LastNameBox.Text = "";
            NationalIdBox.Text = "";
            BirthDatePicker.SelectedDate = null;
            PhoneBox.Text = "";
            EmailBox.Text = "";
            ReportDatePicker.SelectedDate = null;
            IssuedByBox.Text = "";
            MessageBorder.Visibility = Visibility.Collapsed;
        }
    }
}