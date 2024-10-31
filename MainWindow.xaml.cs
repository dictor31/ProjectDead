using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Text.Json;
using WebDead.Model;
using Newtonsoft.Json;

namespace WpfDead
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client = new();

        public ObservableCollection<User> Users { get; set; }
        public User User { get; set; } = new();
        int count = 3;

        public MainWindow()
        {
            InitializeComponent();

            client.BaseAddress = new Uri("https://localhost:7012/api/");
            DataContext = this;
        }

        private async void Enter(object sender, RoutedEventArgs e)
        {
            var responce = await client.GetAsync("DB/GetUsers");
            var responceBody = await responce.Content.ReadAsStringAsync();
            Users = JsonConvert.DeserializeObject<ObservableCollection<User>>(responceBody);
            foreach (var user in Users)
            {
                if (User.Login == string.Empty || User.Password == string.Empty)
                {
                    MessageBox.Show("Заполните поля ввода");
                }
                else if (user.Login == User.Login && user.Password == User.Password && !user.Admin && !user.Ban)
                {
                    MessageBox.Show("Вход успешен");
                    return;
                }
                else if (user.Login == User.Login && user.Password == User.Password && user.Admin && !user.Ban)
                {
                    AdminWindow adminWindow = new AdminWindow();
                    adminWindow.Show();
                    Close();
                    return;
                }
                else if (user.Login == User.Login && user.Ban)
                {
                    MessageBox.Show("Пользователь заблокирован");
                    return;
                }
                else if (user.Login == User.Login && user.Password != User.Password)
                {
                    count -= 1;
                    MessageBox.Show($"Пароль введён неверно. Осталось попыток {count}");
                    if (count < 1)
                    {
                        user.Ban = true;
                        string json = System.Text.Json.JsonSerializer.Serialize(user);
                        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                        var res = await client.PutAsync("DB/PutUser", content);
                        MessageBox.Show("Аккаунт был заблокирован");
                        count = 3;
                    }
                    return;
                }
            }
            MessageBox.Show("Такого пользователя нет");
        }
    }
}