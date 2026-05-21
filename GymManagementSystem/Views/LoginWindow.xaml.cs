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
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorText.Text = "Kullanıcı adı ve şifre boş olamaz!";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            var success = App.AuthService.Login(username, password);

            if (success)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ErrorText.Text = "Kullanıcı adı veya şifre hatalı!";
                ErrorText.Visibility = Visibility.Visible;
            }
        }
    }
}
