using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
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
using WebDead.Model;

namespace WpfDead
{
    /// <summary>
    /// Логика взаимодействия для PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        HttpClient client = new();
        public User User { get; set; }
        public string LastPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordAccess { get; set; }

        public PasswordWindow(User user)
        {
            InitializeComponent();

            client.BaseAddress = new Uri("https://localhost:7012/api/");
            User = user;

            DataContext = this;
        }

        private async void ChangePassword(object sender, RoutedEventArgs e)
        {
            if (LastPassword != User.Password || NewPassword != NewPasswordAccess)
            {
                MessageBox.Show("Введены неверные данные");
                return;
            }
            User.Password = NewPassword;
            User.First = false;
            string json = System.Text.Json.JsonSerializer.Serialize(User);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PutAsync("DB/PutUser", content);
            MessageBox.Show("Пароль успешно изменен");
            Close();
        }
    }
}
